using System;
using System.Collections.Generic;
using System.Linq;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Parameters;

namespace LocalSearchOptimization.Solvers
{
    public class SimulatedAnnealing : IOptimizationAlgorithm
    {
        private SimulatedAnnealingParameters parameters;

        private List<SolutionSummary> solutionsHistory = new List<SolutionSummary>();

        public SimulatedAnnealing(SimulatedAnnealingParameters parameters)
        {
            this.parameters = parameters;
        }

        public IEnumerable<ISolution> Minimize(ISolution solution)
        {
            int iteration = 0;
            Random random = new Random(this.parameters.Seed);
            DateTime startedAt = DateTime.Now;
            ISolution bestSolution = solution;
            ISolution currentSolution = solution;
            solution.IterationNumber = 0;
            solution.TimeInSeconds = 0;
            solution.IsCurrentBest = false;
            solution.IsFinal = false;
            solution.InstanceTag = this.parameters.Name;
            solution.SolutionsHistory = solutionsHistory;
            solutionsHistory.Add(new SolutionSummary
            {
                InstanceTag = this.parameters.Name,
                OperatorTag = currentSolution.OperatorTag,
                IterationNumber = currentSolution.IterationNumber,
                CostValue = currentSolution.CostValue
            });
            Neighborhood neighborhood = this.parameters.UseWeightedNeighborhood ?
                new WeightedNeighborhood(solution, parameters.Operators, parameters.Seed) :
                    new Neighborhood(solution, parameters.Operators, parameters.Seed);
            double temperature = GetStartTemperature(parameters.InitProbability, neighborhood);
            int maxIterationsByTemperature = (int)(parameters.TemperatureLevelPower * neighborhood.Power);
            int iterationsByTemperature = 0;
            int acceptedIterationsByTemperature = 0;
            int frozenState = 0;
            double costDeviation = 0;
            while (frozenState < this.parameters.MaxFrozenLevels && temperature > 10E-5)
            {
                iteration++;
                iterationsByTemperature++;
                ISolution randomNeighbour = neighborhood.GetRandom();
                double costDifference = randomNeighbour.CostValue - currentSolution.CostValue;
                if (costDifference < 0 || (costDifference > 0 && random.NextDouble() < Math.Exp(-costDifference / temperature)))
                {
                    acceptedIterationsByTemperature++;
                    currentSolution = randomNeighbour;
                    currentSolution.IterationNumber = iteration;
                    currentSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                    currentSolution.IsCurrentBest = false;
                    currentSolution.IsFinal = false;
                    currentSolution.InstanceTag = this.parameters.Name;
                    currentSolution.SolutionsHistory = solutionsHistory;
                    solutionsHistory.Add(new SolutionSummary
                    {
                        InstanceTag = this.parameters.Name,
                        OperatorTag = currentSolution.OperatorTag,
                        IterationNumber = currentSolution.IterationNumber,
                        CostValue = currentSolution.CostValue
                    });
                    if (currentSolution.CostValue < bestSolution.CostValue)
                    {
                        yield return bestSolution;
                        currentSolution.IsCurrentBest = true;
                        bestSolution = currentSolution;
                    }
                    else if (parameters.DetailedOutput) yield return currentSolution;
                    neighborhood.MoveToSolution(currentSolution);
                }
                if (iterationsByTemperature >= maxIterationsByTemperature)
                {
                    temperature *= parameters.TemperatureCooling;
                    costDeviation = StandardDeviation(solutionsHistory.GetRange(solutionsHistory.Count - acceptedIterationsByTemperature, acceptedIterationsByTemperature).Select(x => x.CostValue));
                    if (costDeviation <= this.parameters.MinCostDeviation) frozenState++;
                    Console.WriteLine("\tSA {0} cost {1}, temp {2}, accepted {3}, deviation {4}", parameters.Name, currentSolution.CostValue, temperature, acceptedIterationsByTemperature, costDeviation);
                    iterationsByTemperature = 0;
                    acceptedIterationsByTemperature = 0;
                }
            }
            Console.WriteLine("\t{0} finished with cost {1}, temperature {2}, and deviation {3} at iteration {4}", parameters.Name, bestSolution.CostValue, temperature, costDeviation, iteration);
            bestSolution.IterationNumber = iteration;
            bestSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
            bestSolution.IsFinal = true;
            yield return bestSolution;
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

        private double StandardDeviation(IEnumerable<double> values)
        {
            int count = values.Count();
            if (count == 0) return 0;
            double average = values.Average();
            double sqrDiff = values.Sum(x => Math.Pow(x - average, 2));
            return Math.Sqrt(sqrDiff / count);
        }
    }
}