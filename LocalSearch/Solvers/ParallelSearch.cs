using System;
using System.Collections.Generic;
using LocalSearch.Components;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace LocalSearch.Solvers
{
    public class ParallelSearch<S, L, P> : LocalSearch<S>
        where S : ISolution
        where L : LocalSearch<S>
        where P : SearchParameters
    {
        private P parameters;

        private object thisLock = new object();

        public ParallelSearch(P parameters)
        {
            this.parameters = parameters;
            
        }

        public override IEnumerable<S> Minimize(S solution)
        {
            int globalIteration = 0;
            DateTime startedAt = DateTime.Now;
            S bestSolution = solution;
            List<S> solutions = new List<S>();
            P[] parameters = new P[this.parameters.Multistart.InstancesNumber];
            Random random = new Random(this.parameters.Seed);
            for (int i = 0; i < this.parameters.Multistart.InstancesNumber; i++)
            {
                parameters[i] = (P)this.parameters.Clone();
                parameters[i].Name = i.ToString();
                parameters[i].Seed = random.Next();
            }

            Task task = new Task(() =>
            {
                Parallel.For(0, this.parameters.Multistart.InstancesNumber, i =>
                {
                    int iteration = 0;
                    S startSolution = i == 0 ? solution : (S)solution.Shuffle(parameters[i].Seed);
                    L solver = Activator.CreateInstance(typeof(L), new object[] { parameters[i] }) as L;
                    foreach (S s in solver.Minimize(startSolution))
                    {
                        lock (thisLock) solutions.Add(s);
                        iteration = s.IterationNumber;
                    }
                    globalIteration += iteration;
                });
            });

            task.Start();

            while (!task.IsCompleted || solutions.Count() > 0)
            {
                Thread.Sleep(this.parameters.Multistart.OutputDelayInMilliseconds);
                lock (thisLock)
                    if (solutions.Count > 0)
                    {
                        S bestFromList = solutions.OrderBy(x => x.CostValue).First();
                        bestFromList.IsCurrentBest = false;
                        bestFromList.IsFinal = false;
                        if (bestFromList.CostValue < bestSolution.CostValue)
                        {
                            bestFromList.IsCurrentBest = true;
                            yield return bestFromList;
                            bestSolution = bestFromList;
                        }
                        else if (this.parameters.DetailedOutput)
                            yield return bestFromList;
                        solutions.Clear();
                    }
            }

            bestSolution.IterationNumber = globalIteration;
            bestSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
            bestSolution.IsFinal = true;
            yield return bestSolution;
        }
    }
}