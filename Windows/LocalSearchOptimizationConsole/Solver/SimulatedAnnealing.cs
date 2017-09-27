using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Solver
{
    public class SimulatedAnnealing
    {
        private INeighbourhood neighbourhood;
        private Random random = new Random();

        public SimulatedAnnealing(INeighbourhood neighbourhood)
        {
            this.neighbourhood = neighbourhood;
        }

        public IEnumerable<double> Minimize(double initProba = 0.005, double tempDecrease = 0.95, int tempLevelIterations = 10000, int isFrozenIterations = 100000)
        {
            this.neighbourhood.InitRandom();
            ISolution bestSolution = this.neighbourhood.CurrentSolution;
            double bestSolutionCost = bestSolution.GetCostValue();
            ISolution currentSolution = bestSolution;
            double currentSolutionCost = bestSolutionCost;
            double temperature = this.neighbourhood.GetStartTemparature(initProba);
            int isFrozen = 0;
            while (isFrozen < isFrozenIterations)
            {
                for (int i = 0; i < tempLevelIterations; i++)
                {
                    ISolution randomNeighbour = this.neighbourhood.GetRandom();
                    double randomNeighbourCost = randomNeighbour.GetCostValue();
                    double diff = randomNeighbourCost - currentSolutionCost;
                    if (random.NextDouble() < Math.Exp(-diff / temperature))
                    {
                        currentSolution = randomNeighbour;
                        currentSolutionCost = randomNeighbourCost;
                        this.neighbourhood.MoveToSolution(currentSolution);
                        if (diff > 0) isFrozen = 0;
                        if (currentSolutionCost < bestSolutionCost)
                        {
                            bestSolution = currentSolution;
                            bestSolutionCost = currentSolutionCost;
                            yield return bestSolutionCost;
                        }

                        //Console.WriteLine("{0}, {1}, {2}, {3}", currentSolutionCost, bestSolutionCost, temperature, isFrozen);
                    }
                    else
                    {
                        isFrozen++;
                    }
                }
                temperature *= tempDecrease;
            }
        }
    }
}