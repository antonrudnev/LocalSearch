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

        public IEnumerable<ISolution> Minimize(ISolution startSolution)
        {
            stopFlag = false;
            Random random = new Random(parameters.Seed);
            DateTime startedAt = DateTime.Now;
            ISolution bestSolution = startSolution;
            currentSolution = startSolution;
            startSolution.IterationNumber = 0;
            startSolution.TimeInSeconds = 0;
            startSolution.IsCurrentBest = false;
            startSolution.IsFinal = false;
            startSolution.InstanceTag = parameters.Name;
            searchHistory = new List<SolutionSummary>();
            List<ISolution> solutions = new List<ISolution>();
            Task<int>[] solvers = new Task<int>[multistart.InstancesNumber];
            for (int i = 0; i < multistart.InstancesNumber; i++)
            {
                T2 instanceParameters = (T2)parameters.Clone();
                instanceParameters.Name = parameters.Name + (multistart.InstancesNumber > 1 ? ":" + i : null);
                instanceParameters.Seed = random.Next();
                ISolution instanceStartSolution = startSolution.Shuffle(instanceParameters.Seed);
                solvers[i] = Task<int>.Factory.StartNew(() => Solve(instanceParameters, instanceStartSolution, solutions));
            }
            using (ManualResetEventSlim delay = new ManualResetEventSlim(initialState: false))
            {
                while (solvers.Any(x => !x.IsCompleted) || solutions.Count() > 0)
                {
                    delay.Wait(multistart.OutputFrequency);
                    lock (thisLock)
                        if (solutions.Count > 0)
                        {
                            ISolution current = solutions.OrderBy(x => x.CostValue).First();
                            current.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                            current.IsCurrentBest = false;
                            current.IsFinal = false;
                            searchHistory.AddRange(solutions.Select(x => new SolutionSummary
                            {
                                InstanceTag = x.InstanceTag,
                                OperatorTag = x.OperatorTag,
                                IterationNumber = x.IterationNumber,
                                CostValue = x.CostValue
                            }));
                            if (current.CostValue < bestSolution.CostValue)
                            {
                                current.IsCurrentBest = true;
                                currentSolution = current;
                                bestSolution = current;
                                yield return bestSolution;
                            }
                            else if (!multistart.ReturnImprovedOnly)
                            {
                                currentSolution = current;
                                yield return current;
                            }
                            solutions.Clear();
                        }
                }
            }
            bestSolution.IterationNumber = solvers.Sum(x => x.Result);
            bestSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
            bestSolution.IsCurrentBest = true;
            bestSolution.IsFinal = true;
            currentSolution = bestSolution;
            yield return bestSolution;
        }

        public void Stop()
        {
            stopFlag = true;
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