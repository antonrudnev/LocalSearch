using System;
using System.Collections.Generic;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Parameters;

namespace LocalSearchOptimization.Solvers
{
    public class LocalDescent : IOptimizationAlgorithm
    {
        private LocalDescentParameters parameters;

        private ISolution currentSolution;

        private List<SolutionSummary> searchHistory;

        private bool stopFlag = false;

        public ISolution CurrentSolution { get => currentSolution; }

        public List<SolutionSummary> SearchHistory { get => searchHistory; }

        public LocalDescent(LocalDescentParameters parameters)
        {
            this.parameters = parameters;
        }

        public IEnumerable<ISolution> Minimize(ISolution startSolution)
        {
            stopFlag = false;
            int iteration = 0;
            DateTime startedAt = DateTime.Now;
            currentSolution = startSolution;
            startSolution.IterationNumber = 0;
            startSolution.TimeInSeconds = 0;
            startSolution.IsCurrentBest = true;
            startSolution.IsFinal = false;
            startSolution.InstanceTag = parameters.Name;
            searchHistory = new List<SolutionSummary>
            {
                new SolutionSummary
                {
                    InstanceTag = parameters.Name,
                    OperatorTag = currentSolution.OperatorTag,
                    IterationNumber = currentSolution.IterationNumber,
                    CostValue = currentSolution.CostValue
                }
            };
            Neighborhood neighborhood = new Neighborhood(currentSolution, parameters.Operators, parameters.Seed);
            bool bestFound;
            do
            {
                bestFound = true;
                foreach (ISolution neighbor in neighborhood.Neighbors)
                {
                    iteration++;
                    if (neighbor.CostValue < currentSolution.CostValue)
                    {
                        currentSolution = neighbor;
                        bestFound = false;
                        if (!parameters.IsSteepestDescent) break;
                    }
                }
                if (!bestFound)
                {
                    currentSolution.IterationNumber = iteration;
                    currentSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                    currentSolution.IsCurrentBest = true;
                    currentSolution.IsFinal = false;
                    currentSolution.InstanceTag = parameters.Name;
                    searchHistory.Add(new SolutionSummary
                    {
                        InstanceTag = parameters.Name,
                        OperatorTag = currentSolution.OperatorTag,
                        IterationNumber = currentSolution.IterationNumber,
                        CostValue = currentSolution.CostValue
                    });
                    if (parameters.DetailedOutput) yield return currentSolution;
                    neighborhood.MoveToSolution(currentSolution);
                }
            } while (!(bestFound || stopFlag));
            currentSolution.IterationNumber = iteration;
            currentSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
            currentSolution.IsFinal = true;
            yield return currentSolution;
            Console.WriteLine("{0} finished with cost {1} at iteration {2}", parameters.Name, currentSolution.CostValue, iteration);
        }

        public void Stop()
        {
            stopFlag = true;
        }
    }
}