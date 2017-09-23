using System;
using System.Collections.Generic;
using LocalSearch.Components;

namespace LocalSearch.Solvers
{
    public class LocalDescent<T> : LocalSearch<T>
        where T : ISolution
    {
        private LocalDescentParameters parameters;

        public LocalDescent(LocalDescentParameters parameters)
        {
            this.parameters = parameters;
        }

        public override IEnumerable<T> Minimize(T solution)
        {
            solution.IsCurrentBest = true;
            solution.IsFinal = false;
            int iteration = 0;
            DateTime startedAt = DateTime.Now;
            ISolution bestNeighbor = solution;
            ISolution subOptimal = solution;
            Neighborhood neighborhood = new Neighborhood(solution, parameters.Operations, parameters.Seed);
            bool bestFound = false;
            while (!bestFound)
            {
                foreach (ISolution neighbor in neighborhood.Neighbors)
                {
                    iteration++;
                    if (neighbor.CostValue < bestNeighbor.CostValue)
                    {
                        bestNeighbor = neighbor;
                        if (!parameters.IsSteepestDescent) break;
                    }
                }
                if (bestNeighbor.CostValue < subOptimal.CostValue)
                {
                    bestNeighbor.IterationNumber = iteration;
                    bestNeighbor.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                    bestNeighbor.IsCurrentBest = true;
                    bestNeighbor.IsFinal = false;
                    neighborhood.MoveToSolution(bestNeighbor);
                    if (parameters.DetailedOutput || parameters.Multistart != null) yield return (T)subOptimal;
                    subOptimal = bestNeighbor;
                }
                else
                {
                    subOptimal.IsFinal = true;
                    yield return (T)subOptimal;
                    bestFound = true;
                }
            }
            Console.WriteLine("\tLD {0} finished with cost {1} at iteration {2}", parameters.Name, subOptimal.CostValue, iteration);
        }
    }
}