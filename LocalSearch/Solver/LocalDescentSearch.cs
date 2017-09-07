using System;
using System.Collections.Generic;

namespace LocalSearch.Solver
{
    public class LocalDescentSearch
    {
        private INeighbourhood neighbourhood;

        public LocalDescentSearch(INeighbourhood neighbourhood)
        {
            this.neighbourhood = neighbourhood;
        }

        public IEnumerable<SolutionDetails> Minimize(int multistart = 1, bool acceptFirst = true)
        {
            int iteration = 0;
            DateTime startedAt = DateTime.Now;
            ISolution bestSolution;
            SolutionDetails bestSolutionDetails = new SolutionDetails() { CostValue = double.MaxValue };
            for (int i = 1; i <= multistart; i++)
            {
                this.neighbourhood.InitToRandom();
                ISolution subOptimalSolution = this.neighbourhood.CurrentSolution;
                SolutionDetails subOptimalSolutionDetails = subOptimalSolution.GetDetails();
                ISolution bestNeighbour = subOptimalSolution;
                SolutionDetails bestNeighbourDetails = subOptimalSolutionDetails;
                bool bestFound = false;
                while (!bestFound)
                {
                    while (!this.neighbourhood.IsScanned)
                    {
                        iteration++;
                        ISolution neighbour = this.neighbourhood.GetNext();
                        SolutionDetails neighbourDetails = neighbour.GetDetails();
                        if (neighbourDetails.CostValue < bestNeighbourDetails.CostValue)
                        {
                            bestNeighbour = neighbour;
                            bestNeighbourDetails = neighbourDetails;
                            if (acceptFirst) break;
                        }
                    }
                    if (bestNeighbourDetails.CostValue < subOptimalSolutionDetails.CostValue)
                    {
                        subOptimalSolutionDetails = bestNeighbourDetails;
                        subOptimalSolution = bestNeighbour;
                        this.neighbourhood.MoveToSolution(subOptimalSolution);
                        subOptimalSolutionDetails.Iteration = iteration;
                        subOptimalSolutionDetails.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                        subOptimalSolutionDetails.IsBest = false;
                        yield return subOptimalSolutionDetails;
                    }
                    else
                    {
                        bestFound = true;
                    }
                }

                if (subOptimalSolutionDetails.CostValue < bestSolutionDetails.CostValue)
                {
                    bestSolutionDetails = subOptimalSolutionDetails;
                    bestSolution = subOptimalSolution;
                    bestSolutionDetails.IsBest = true;
                    yield return bestSolutionDetails;
                }
            }
            yield return bestSolutionDetails;
        }
    }
}