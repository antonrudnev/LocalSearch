using System;
using System.Collections.Generic;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Parameters;

namespace LocalSearchOptimization.Solvers
{
    public class LocalDescent : IOptimizationAlgorithm
    {
        private LocalDescentParameters parameters;

        private List<Tuple<string, int, double>> solutionsHistory = new List<Tuple<string, int, double>>();

        public LocalDescent(LocalDescentParameters parameters)
        {
            this.parameters = parameters;
        }

        public IEnumerable<ISolution> Minimize(ISolution solution)
        {
            int iteration = 0;
            DateTime startedAt = DateTime.Now;
            ISolution bestSolution = solution;
            ISolution currentSolution = solution;
            solution.IterationNumber = 0;
            solution.TimeInSeconds = 0;
            solution.IsCurrentBest = true;
            solution.IsFinal = false;
            solution.InstanceTag = this.parameters.Name;
            solution.SolutionsHistory = solutionsHistory;
            solutionsHistory.Add(new Tuple<string, int, double>(this.parameters.Name, currentSolution.IterationNumber, currentSolution.CostValue));
            Neighborhood neighborhood = new Neighborhood(currentSolution, parameters.Operators, parameters.Seed);
            bool bestFound = false;
            while (!bestFound)
            {
                foreach (ISolution neighbor in neighborhood.Neighbors)
                {
                    iteration++;
                    if (neighbor.CostValue < currentSolution.CostValue)
                    {
                        currentSolution = neighbor;
                        if (!parameters.IsSteepestDescent) break;
                    }
                }
                if (currentSolution.CostValue < bestSolution.CostValue)
                {
                    if (parameters.DetailedOutput) yield return bestSolution;
                    currentSolution.IterationNumber = iteration;
                    currentSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                    currentSolution.IsCurrentBest = true;
                    currentSolution.IsFinal = false;
                    currentSolution.InstanceTag = this.parameters.Name;
                    currentSolution.SolutionsHistory = solutionsHistory;
                    solutionsHistory.Add(new Tuple<string, int, double>(this.parameters.Name, currentSolution.IterationNumber, currentSolution.CostValue));
                    neighborhood.MoveToSolution(currentSolution);
                    bestSolution = currentSolution;
                }
                else
                {
                    bestFound = true;
                    bestSolution.IsFinal = true;
                    yield return bestSolution;
                }
            }
            Console.WriteLine("\tLD {0} finished with cost {1} at iteration {2}", parameters.Name, bestSolution.CostValue, iteration);
        }
    }
}