using System;
using System.Collections.Generic;

namespace LocalSearch.Solver
{
    public class SimulatedAnnealing
    {
        private INeighbourhood neighbourhood;
        private Random random = new Random();

        public SimulatedAnnealing(INeighbourhood neighbourhood)
        {
            this.neighbourhood = neighbourhood;
        }

        public IEnumerable<SolutionDetails> Minimize()
        {
            int iteration = 0;
            DateTime startedAt = DateTime.Now;

            double initProba = this.neighbourhood.initProba;
            double tempDecrease = this.neighbourhood.tempDecrease;
            this.neighbourhood.InitToRandom();
            int tempLevelIterations = this.neighbourhood.tempLevelIterations;
            int frozenRate = this.neighbourhood.frozenRate;
            int isLocalOptimaIterations = this.neighbourhood.isLocalOptimaIterations;
            ISolution bestSolution = this.neighbourhood.CurrentSolution;
            SolutionDetails bestSolutionDetails = bestSolution.GetDetails();
            ISolution currentSolution = bestSolution;
            SolutionDetails currentSolutionDetails = bestSolutionDetails;
            double temperature = GetStartTemparature(initProba);
            int acceptedIterations = frozenRate;
            int isLocalOptima = 0;
            while ((acceptedIterations >= frozenRate || isLocalOptima < isLocalOptimaIterations))
            {
                acceptedIterations = 0;
                for (int i = 0; i < tempLevelIterations; i++)
                {
                    iteration++;
                    ISolution randomNeighbour = this.neighbourhood.GetRandom();
                    SolutionDetails randomNeighbourDetails = randomNeighbour.GetDetails();
                    double diff = randomNeighbourDetails.CostValue - currentSolutionDetails.CostValue;
                    if (random.NextDouble() < Math.Exp(-diff / temperature))
                    {
                        acceptedIterations++;
                        currentSolution = randomNeighbour;
                        currentSolutionDetails = randomNeighbourDetails;
                        currentSolutionDetails.Iteration = iteration;
                        currentSolutionDetails.IsBest = false;
                        currentSolutionDetails.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                        if (isLocalOptima % 1000 == 0) yield return currentSolutionDetails;
                        this.neighbourhood.MoveToSolution(currentSolution);
                        if (currentSolutionDetails.CostValue < bestSolutionDetails.CostValue)
                        {
                            isLocalOptima = 0;
                            bestSolution = currentSolution;
                            bestSolutionDetails = currentSolutionDetails;
                            bestSolutionDetails.IsBest = true;
                            yield return bestSolutionDetails;
                        }
                        else
                        {
                            isLocalOptima++;
                        }
                    }
                }
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}", currentSolutionDetails.CostValue, bestSolutionDetails.CostValue, temperature, iteration, acceptedIterations - frozenRate, isLocalOptimaIterations - isLocalOptima);
                temperature *= tempDecrease;
            }
            Console.WriteLine("Finished SA");
        }

        private double GetStartTemparature(double initProba)
        {
            double maxCost = 0;
            this.neighbourhood.Reset();
            while (!this.neighbourhood.IsScanned)
            {
                ISolution neighbour = this.neighbourhood.GetNext();
                double neighbourCost = neighbour.GetDetails().CostValue;
                if (neighbourCost > maxCost) maxCost = neighbourCost;
            }
            return -(maxCost - this.neighbourhood.CurrentSolution.GetDetails().CostValue) / Math.Log(initProba);
        }
    }
}