using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Parameters;

namespace LocalSearchOptimization.Solvers
{
    public class ParallelMultistart<T1, T2> : IOptimizationAlgorithm
        where T1 : IOptimizationAlgorithm
        where T2 : CoreParameters
    {
        private T2 parameters;

        private MultistartOptions multistart;

        private List<ISolution> solutionsHistory = new List<ISolution>();

        private object thisLock = new object();

        public ParallelMultistart(T2 parameters, MultistartOptions multistart)
        {
            this.parameters = parameters;
            this.multistart = multistart;
        }

        public IEnumerable<ISolution> Minimize(ISolution solution)
        {
            DateTime startedAt = DateTime.Now;
            ISolution bestSolution = solution;
            List<ISolution> solutions = new List<ISolution>();
            Random random = new Random(this.parameters.Seed);
            Task<int>[] solvers = new Task<int>[this.multistart.InstancesNumber];
            for (int i = 0; i < this.multistart.InstancesNumber; i++)
            {
                T2 instanceParameters = (T2)this.parameters.Clone();
                instanceParameters.Name = i.ToString();
                instanceParameters.Seed = random.Next();
                ISolution startSolution = i == 0 ? solution : solution.Shuffle(instanceParameters.Seed);
                solvers[i] = Task<int>.Factory.StartNew(() => Solve(instanceParameters, startSolution, solutions));
            }
            using (ManualResetEventSlim delay = new ManualResetEventSlim(initialState: false))
            {
                while (solvers.Any(x => !x.IsCompleted) || solutions.Count() > 0)
                {
                    delay.Wait(this.multistart.OutputFrequency);
                    lock (thisLock)
                        if (solutions.Count > 0)
                        {
                            ISolution bestFromList = solutions.OrderBy(x => x.CostValue).First();
                            bestFromList.IsCurrentBest = false;
                            bestFromList.IsFinal = false;
                            bestFromList.SolutionsHistory = solutionsHistory;
                            solutionsHistory.AddRange(solutions);
                            if (bestFromList.CostValue < bestSolution.CostValue)
                            {
                                bestFromList.IsCurrentBest = true;
                                yield return bestFromList;
                                bestSolution = bestFromList;
                            }
                            else if (!this.multistart.ReturnImprovedOnly)
                                yield return bestFromList;
                            solutions.Clear();
                        }
                }
            }
            bestSolution.IterationNumber = solvers.Sum(x => x.Result);
            bestSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
            bestSolution.IsFinal = true;
            yield return bestSolution;
        }

        private int Solve(T2 parameters, ISolution startSolution, List<ISolution> output)
        {
            int iteration = 0;
            T1 solver = (T1)Activator.CreateInstance(typeof(T1), new object[] { parameters });
            foreach (ISolution solution in solver.Minimize(startSolution))
            {
                lock (thisLock) output.Add(solution);
                iteration = solution.IterationNumber;
            }
            return iteration;
        }
    }
}