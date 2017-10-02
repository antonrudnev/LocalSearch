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

        private ISolution currentSolution;

        private List<SolutionSummary> searchHistory;

        private bool stopFlag = false;

        public ISolution CurrentSolution { get => currentSolution; }

        public List<SolutionSummary> SearchHistory { get => searchHistory; }

        private object thisLock = new object();

        public ParallelMultistart(T2 parameters, MultistartOptions multistart)
        {
            this.parameters = parameters;
            this.multistart = multistart;
        }

        public IEnumerable<ISolution> Minimize(ISolution solution)
        {
            this.stopFlag = false;
            Random random = new Random(this.parameters.Seed);
            DateTime startedAt = DateTime.Now;
            ISolution bestSolution = solution;
            this.currentSolution = solution;
            solution.IterationNumber = 0;
            solution.TimeInSeconds = 0;
            solution.IsCurrentBest = false;
            solution.IsFinal = false;
            solution.InstanceTag = this.parameters.Name;
            searchHistory = new List<SolutionSummary>();
            List<ISolution> solutions = new List<ISolution>();
            Task<int>[] solvers = new Task<int>[this.multistart.InstancesNumber];
            for (int i = 0; i < this.multistart.InstancesNumber; i++)
            {
                T2 instanceParameters = (T2)this.parameters.Clone();
                instanceParameters.Name = this.parameters.Name + (this.multistart.InstancesNumber > 1 ? ":" + i : null);
                instanceParameters.Seed = random.Next();
                ISolution startSolution = solution.Shuffle(instanceParameters.Seed);
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
                            ISolution currentSolution = solutions.OrderBy(x => x.CostValue).First();
                            currentSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                            currentSolution.IsCurrentBest = false;
                            currentSolution.IsFinal = false;
                            searchHistory.AddRange(solutions.Select(x => new SolutionSummary
                            {
                                InstanceTag = x.InstanceTag,
                                OperatorTag = x.OperatorTag,
                                IterationNumber = x.IterationNumber,
                                CostValue = x.CostValue
                            }));
                            if (currentSolution.CostValue < bestSolution.CostValue)
                            {
                                currentSolution.IsCurrentBest = true;
                                bestSolution = currentSolution;
                                yield return bestSolution;
                                this.currentSolution = bestSolution;
                            }
                            else if (!this.multistart.ReturnImprovedOnly)
                            {
                                yield return currentSolution;
                                this.currentSolution = currentSolution;
                            }
                            solutions.Clear();
                        }
                }
            }
            bestSolution.IterationNumber = solvers.Sum(x => x.Result);
            bestSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
            bestSolution.IsFinal = true;
            yield return bestSolution;
            this.currentSolution = bestSolution;
        }

        public void Stop()
        {
            this.stopFlag = true;
        }

        private int Solve(T2 parameters, ISolution startSolution, List<ISolution> output)
        {
            int iteration = 0;
            T1 solver = (T1)Activator.CreateInstance(typeof(T1), new object[] { parameters });
            foreach (ISolution solution in solver.Minimize(startSolution))
            {
                lock (thisLock) output.Add(solution);
                iteration = solution.IterationNumber;
                if (stopFlag) solver.Stop();
            }
            return iteration;
        }
    }
}