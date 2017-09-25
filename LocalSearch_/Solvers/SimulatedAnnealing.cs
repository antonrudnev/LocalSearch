using System;
using System.Collections.Generic;
using System.Linq;
using LocalSearch.Components;

namespace LocalSearch.Solvers
{
    public class SimulatedAnnealing<T> : LocalSearch<T> where T : ISolution
    {
        private ParametersSimulatedAnnealing parameters;

        public SimulatedAnnealing(ParametersSimulatedAnnealing parameters)
        {
            this.parameters = parameters;
        }

        public override IEnumerable<T> Minimize(T solution)
        {
            int iteration = 0;
            Random random = new Random(this.parameters.Seed);
            DateTime startedAt = DateTime.Now;
            ISolution bestSolution = solution;
            ISolution currentSolution = solution;
            Neighborhood neighborhood = this.parameters.WeightNeighborhood ?
                new NeighborhoodWeighted(solution, parameters.Operations, parameters.Seed) :
                    new Neighborhood(solution, parameters.Operations, parameters.Seed);
            double temperature = GetStartTemperature(parameters.InitProbability, neighborhood);
            int maxIterationsForTemperatureValue = (int)(parameters.TemperatureLevelPasses * neighborhood.Power);
            int maxIterationsSinceLastTransition = (int)(parameters.MaxPassesSinceLastTransition * neighborhood.Power);
            int iterationsForTemperatureValue = 0;
            int iterationsSinceLastTransition = 0;
            while (iterationsSinceLastTransition < maxIterationsSinceLastTransition)
            {
                iteration++;
                iterationsForTemperatureValue++;
                iterationsSinceLastTransition++;
                ISolution randomNeighbour = neighborhood.GetRandom();
                double costDifference = randomNeighbour.CostValue - currentSolution.CostValue;
                if (costDifference < 0 || (costDifference > 0 && random.NextDouble() < Math.Exp(-costDifference / temperature)))
                {
                    iterationsSinceLastTransition = 0;
                    currentSolution = randomNeighbour;
                    currentSolution.IterationNumber = iteration;
                    currentSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                    currentSolution.IsCurrentBest = false;
                    currentSolution.IsFinal = false;
                    if (parameters.DetailedOutput) yield return (T)currentSolution;
                    if (currentSolution.CostValue < bestSolution.CostValue)
                    {
                        currentSolution.IsCurrentBest = true;
                        yield return (T)currentSolution;
                        bestSolution = currentSolution;
                    }
                    neighborhood.MoveToSolution(currentSolution);
                }
                if (iterationsForTemperatureValue >= maxIterationsForTemperatureValue)
                {
                    iterationsForTemperatureValue = 0;
                    temperature *= parameters.TemperatureCooling;
                    //Console.WriteLine("\tSA {0} cost {1}, temp {2}, iter {3}, reserve {4}", parameters.Name, currentSolution.CostValue, temperature, iteration, maxIterationsSinceLastTransition - iterationsSinceLastTransition);
                }
            }
            Console.WriteLine("\tSA {0} finished with cost {1} and temperature {2} at iteration {3}", parameters.Name, bestSolution.CostValue, temperature, iteration);
            bestSolution.IterationNumber = iteration;
            bestSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
            bestSolution.IsFinal = true;
            yield return (T)bestSolution;
        }

        private double GetStartTemperature(double initProbability, Neighborhood neighborhood)
        {
            List<double> temperature = new List<double>();
            foreach (ISolution solution in neighborhood.Neighbors)
            {
                if (solution.CostValue > neighborhood.CurrentSolution.CostValue)
                    temperature.Add(-(solution.CostValue - neighborhood.CurrentSolution.CostValue) / Math.Log(initProbability));
            }
            return Percentile(temperature, Math.Sqrt(initProbability));
        }

        private double Percentile(IEnumerable<double> sequence, double percentile)
        {
            double[] sorted = sequence.OrderBy(x => x).ToArray();
            int N = sorted.Length;
            double n = (N - 1) * percentile + 1;
            // Another method: double n = (N + 1) * percentile;
            if (n == 1d) return sorted[0];
            else if (n == N) return sorted[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sorted[k - 1] + d * (sorted[k] - sorted[k - 1]);
            }
        }
    }
}