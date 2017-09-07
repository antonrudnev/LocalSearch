using System;

namespace TestConsole.Solver
{
    public class LocalDescentSearch
    {
        private INeighbourhood neighbourhood;

        public LocalDescentSearch(INeighbourhood neighbourhood)
        {
            this.neighbourhood = neighbourhood;
        }

        public void Minimize(int multistart = 1)
        {
            ISolution bestSolution;
            double bestSolutionCost = double.MaxValue;
            for (int i = 1; i <= multistart; i++)
            {
                this.neighbourhood.InitRandom();
                ISolution subOptimalSolution = this.neighbourhood.CurrentSolution;
                double subOptimalSolutionCost = subOptimalSolution.GetCostValue();
                ISolution bestNeighbour = subOptimalSolution;
                double bestNeighbourCost = subOptimalSolutionCost;
                bool bestFound = false;
                while (!bestFound)
                {
                    while (!this.neighbourhood.IsScanned)
                    {
                        ISolution neighbour = this.neighbourhood.GetNext();
                        double neighbourCost = neighbour.GetCostValue();
                        if (neighbourCost < bestNeighbourCost)
                        {
                            bestNeighbour = neighbour;
                            bestNeighbourCost = neighbourCost;
                        }
                    }

                    if (bestNeighbourCost < subOptimalSolutionCost)
                    {
                        subOptimalSolutionCost = bestNeighbourCost;
                        subOptimalSolution = bestNeighbour;
                        this.neighbourhood.MoveToSolution(subOptimalSolution);
                    }
                    else
                    {
                        bestFound = true;
                    }
                }

                if (subOptimalSolutionCost < bestSolutionCost)
                {
                    bestSolutionCost = subOptimalSolutionCost;
                    bestSolution = subOptimalSolution;
                    Console.WriteLine(bestSolutionCost);
                }
            }
        }
    }
}