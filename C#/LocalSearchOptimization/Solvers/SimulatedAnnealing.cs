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

        private ISolution currentSolution;

        private List<SolutionSummary> searchHistory;

        private bool stopFlag = false;

        public ISolution CurrentSolution { get => currentSolution; }

        public List<SolutionSummary> SearchHistory { get => searchHistory; }

        public SimulatedAnnealing(SimulatedAnnealingParameters parameters)
        {
            this.parameters = parameters;
        }

        public IEnumerable<ISolution> Minimize(ISolution startSolution)
        {
            stopFlag = false;
            int iteration = 0;
            Random random = new Random(parameters.Seed);
            DateTime startedAt = DateTime.Now;
            ISolution bestSolution = startSolution;
            ISolution current = startSolution;
            currentSolution = startSolution;
            startSolution.IterationNumber = 0;
            startSolution.TimeInSeconds = 0;
            startSolution.IsCurrentBest = false;
            startSolution.IsFinal = false;
            startSolution.InstanceTag = parameters.Name;
            searchHistory = new List<SolutionSummary>
            {
                new SolutionSummary
                {
                    InstanceTag = parameters.Name,
                    OperatorTag = current.OperatorTag,
                    IterationNumber = current.IterationNumber,
                    CostValue = current.CostValue
                }
            };
            Neighborhood neighborhood = parameters.UseWeightedNeighborhood ?
                new NeighborhoodWeighted(startSolution, parameters.Operators, parameters.Seed) :
                    new Neighborhood(startSolution, parameters.Operators, parameters.Seed);
            double temperature = GetStartTemperature(parameters.InitProbability, neighborhood);
            int maxIterationsByTemperature = (int)(parameters.TemperatureLevelPower * neighborhood.Power);
            int iterationsByTemperature = 0;
            int acceptedIterationsByTemperature = 0;
            int frozenState = 0;
            double costDeviation = 0;
            while (!stopFlag && frozenState < parameters.MaxFrozenLevels && temperature > 10E-5)
            {
                iteration++;
                iterationsByTemperature++;
                ISolution randomNeighbour = neighborhood.GetRandom();
                double costDifference = randomNeighbour.CostValue - current.CostValue;
                if (costDifference < 0 || (costDifference > 0 && random.NextDouble() < Math.Exp(-costDifference / temperature)))
                {
                    acceptedIterationsByTemperature++;
                    current = randomNeighbour;
                    current.IterationNumber = iteration;
                    current.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                    current.IsCurrentBest = false;
                    current.IsFinal = false;
                    current.InstanceTag = parameters.Name;
                    searchHistory.Add(new SolutionSummary
                    {
                        InstanceTag = parameters.Name,
                        OperatorTag = current.OperatorTag,
                        IterationNumber = current.IterationNumber,
                        CostValue = current.CostValue
                    });
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
                        yield return current;
                    }
                    neighborhood.MoveToSolution(current);
                }
                if (iterationsByTemperature >= maxIterationsByTemperature)
                {
                    temperature *= parameters.TemperatureCooling;
                    costDeviation = StandardDeviation(searchHistory.GetRange(searchHistory.Count - acceptedIterationsByTemperature, acceptedIterationsByTemperature).Select(x => x.CostValue));
                    if (costDeviation <= parameters.MinCostDeviation)
                        frozenState++;
                    else
                        frozenState = 0;
                    Console.WriteLine("{0} cost {1}, temp {2}, accepted {3}, deviation {4}, time {5:F2}s", parameters.Name, current.CostValue, temperature, acceptedIterationsByTemperature, costDeviation, current.TimeInSeconds);
                    iterationsByTemperature = 0;
                    acceptedIterationsByTemperature = 0;
                    current = current.Transcode();
                    neighborhood.MoveToSolution(current);
                }
            }
            Console.WriteLine("{0} finished with cost {1}, temperature {2}, and deviation {3} at iteration {4}, time {5:F2}s", parameters.Name, bestSolution.CostValue, temperature, costDeviation, iteration, bestSolution.TimeInSeconds);
            bestSolution.IterationNumber = iteration;
            bestSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
            bestSolution.IsFinal = true;
            currentSolution = bestSolution;
            yield return bestSolution;
        }

        public void Stop()
        {
            stopFlag = true;
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