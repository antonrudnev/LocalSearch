using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Parameters;

namespace LocalSearchOptimization.Solvers
{
    public class ParallelMultistart : IOptimizationAlgorithm
    {
        private MultistartParameters parameters;

        private ISolution currentSolution;

        private List<SolutionSummary> searchHistory;

        private bool stopFlag = false;

        public ISolution CurrentSolution { get => currentSolution; }

        public List<SolutionSummary> SearchHistory { get => searchHistory; }

        private object thisLock = new object();

        public ParallelMultistart(MultistartParameters parameters)
        {
            this.parameters = parameters;
        }

        public IEnumerable<ISolution> Minimize(ISolution startSolution)
        {
            stopFlag = false;
            OptimizationParameters coreParameters = parameters.Parameters;
            Random random = new Random(parameters.Seed);
            DateTime startedAt = DateTime.Now;
            ISolution bestSolution = startSolution;
            currentSolution = startSolution;
            startSolution.IterationNumber = 0;
            startSolution.TimeInSeconds = 0;
            startSolution.IsCurrentBest = false;
            startSolution.IsFinal = false;
            startSolution.InstanceTag = coreParameters.Name;
            searchHistory = new List<SolutionSummary>();
            List<ISolution> solutions = new List<ISolution>();
            Task<int>[] solvers = new Task<int>[parameters.InstancesNumber];
            for (int i = 0; i < parameters.InstancesNumber; i++)
            {
                OptimizationParameters instanceParameters = coreParameters.Clone();
                instanceParameters.Name = (parameters.InstancesNumber > 1 ? parameters.Name + "|" + i + "|" : "") + coreParameters.Name;
                instanceParameters.Seed = random.Next();
                ISolution instanceStartSolution = parameters.RandomizeStart ? startSolution.Shuffle(instanceParameters.Seed) : startSolution;
                solvers[i] = Task<int>.Factory.StartNew(() => Solve(parameters.OptimizationAlgorithm, instanceParameters, instanceStartSolution, solutions));
            }
            using (ManualResetEventSlim delay = new ManualResetEventSlim(initialState: false))
            {
                while (solvers.Any(x => !x.IsCompleted) || solutions.Count() > 0)
                {
                    delay.Wait(parameters.OutputFrequency);
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
                            else if (parameters.DetailedOutput)
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
            yield return currentSolution;
        }

        public void Stop()
        {
            stopFlag = true;
        }

        private int Solve(Type optimizationAlgorithm, OptimizationParameters parameters, ISolution startSolution, List<ISolution> output)
        {
            int iteration = 0;
            IOptimizationAlgorithm solver = (IOptimizationAlgorithm)Activator.CreateInstance(optimizationAlgorithm, new object[] { parameters });
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