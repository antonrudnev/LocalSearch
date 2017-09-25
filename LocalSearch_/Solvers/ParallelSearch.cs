using System;
using System.Collections.Generic;
using LocalSearch.Components;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace LocalSearch.Solvers
{
    public class ParallelSearch<T1, T2, T3> : LocalSearch<T1>
        where T1 : ISolution
        where T2 : LocalSearch<T1>
        where T3 : SearchParameters
    {
        private T3 parameters;

        private object thisLock = new object();

        public ParallelSearch(T3 parameters)
        {
            this.parameters = parameters;

        }

        public override IEnumerable<T1> Minimize(T1 solution)
        {
            DateTime startedAt = DateTime.Now;
            T1 bestSolution = solution;
            List<T1> solutions = new List<T1>();
            Random random = new Random(this.parameters.Seed);
            Task<int>[] tasks = new Task<int>[this.parameters.Multistart.InstancesNumber];
            for (int i = 0; i < this.parameters.Multistart.InstancesNumber; i++)
            {
                T3 instanceParameters = (T3)this.parameters.Clone();
                instanceParameters.Name = i.ToString();
                instanceParameters.Seed = random.Next();
                T1 startSolution = i == 0 ? solution : (T1)solution.Shuffle(instanceParameters.Seed);
                tasks[i] = Task<int>.Factory.StartNew(() => Solve(instanceParameters, startSolution, solutions));
            }

            while (!tasks.All(x => x.IsCompleted) || solutions.Count() > 0)
            {
                Task.Delay(this.parameters.Multistart.OutputDelayInMilliseconds).Wait();
                lock (thisLock)
                    if (solutions.Count > 0)
                    {
                        T1 bestFromList = solutions.OrderBy(x => x.CostValue).First();
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

            bestSolution.IterationNumber = tasks.Sum(x => x.Result);
            bestSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
            bestSolution.IsFinal = true;
            yield return bestSolution;
        }

        private int Solve(T3 parameters, T1 startSolution, List<T1> solutionsOutput)
        {
            int iteration = 0;
            T2 solver = Activator.CreateInstance(typeof(T2), new object[] { parameters }) as T2;
            foreach (T1 solution in solver.Minimize(startSolution))
            {
                lock (thisLock) solutionsOutput.Add(solution);
                iteration = solution.IterationNumber;
            }
            return iteration;
        }
    }
}