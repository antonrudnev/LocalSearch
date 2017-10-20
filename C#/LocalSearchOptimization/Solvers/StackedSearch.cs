using LocalSearchOptimization.Components;
using LocalSearchOptimization.Parameters;
using System;
using System.Collections.Generic;

namespace LocalSearchOptimization.Solvers
{
    public class StackedSearch : IOptimizationAlgorithm
    {
        private StackedParameters parameters;

        private ISolution currentSolution;

        private List<SolutionSummary> searchHistory;

        private bool stopFlag = false;

        public ISolution CurrentSolution { get => currentSolution; }

        public List<SolutionSummary> SearchHistory { get => searchHistory; }

        public StackedSearch(StackedParameters parameters)
        {
            this.parameters = parameters;
        }

        public IEnumerable<ISolution> Minimize(ISolution startSolution)
        {
            stopFlag = false;
            int globalIteration = 0;
            DateTime startedAt = DateTime.Now;
            ISolution bestSolution = startSolution;
            currentSolution = startSolution;
            startSolution.IterationNumber = 0;
            startSolution.TimeInSeconds = 0;
            startSolution.IsCurrentBest = false;
            startSolution.IsFinal = false;
            startSolution.InstanceTag = parameters.Name;
            searchHistory = new List<SolutionSummary>();
            for (int i = 0; i < parameters.OptimizationAlgorithms.Length; i++)
            {
                OptimizationParameters componentParameters = parameters.Parameters[i].Clone();
                componentParameters.Name = parameters.Name + ":" + i + ":" + componentParameters.Name;
                componentParameters.Seed = parameters.Seed;
                IOptimizationAlgorithm solver = (IOptimizationAlgorithm)Activator.CreateInstance(parameters.OptimizationAlgorithms[i], new object[] { componentParameters });
                int iteration = 0;
                foreach (ISolution current in solver.Minimize(currentSolution))
                {
                    iteration = current.IterationNumber;
                    current.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                    current.IsCurrentBest = false;
                    current.IsFinal = false;
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
                        yield return currentSolution;
                    }
                    if (stopFlag) solver.Stop();
                }
                searchHistory.AddRange(solver.SearchHistory);
                globalIteration += iteration;
                if (stopFlag) break;
            }
            bestSolution.IterationNumber = globalIteration;
            bestSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
            bestSolution.IsFinal = true;
            currentSolution = bestSolution;
            yield return bestSolution;
        }

        public void Stop()
        {
            stopFlag = true;
        }
    }
}