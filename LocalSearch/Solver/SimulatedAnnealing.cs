using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using LocalSearch.Components;

namespace LocalSearch.Solver
{
    public class SimulatedAnnealing<T> where T : ISolution
    {
        private List<Operation> operations;

        private Random init = new Random();
        private Object thisLock = new Object();

        public SimulatedAnnealing(List<Operation> operations)
        {
            this.operations = operations;
        }

        public IEnumerable<T> Minimize(T solution, SimulatedAnnealingParameters parameters)
        {
            int globalIteration = 0;
            DateTime startedAt = DateTime.Now;
            ISolution globalBestSolution = solution;
            Queue<ISolution> solutionsQueue = new Queue<ISolution>();
            List<ISolution> bestList = new List<ISolution>();

            Task task = new Task(() =>
            {
                Parallel.For(0, parameters.Multistart, i =>
                {
                    Random random;
                    lock (thisLock) random = new Random(this.init.Next());
                    int iteration = 0;
                    ISolution bestSolution = solution;
                    ISolution currentSolution = solution;
                    Neighborhood neighborhood = new Neighborhood(solution, this.operations);
                    double temperature = GetStartTemperature(parameters.InitProbability, neighborhood);
                    int maxIterationsForTemperatureValue = (int)(parameters.TemperatureLevelPasses * neighborhood.Power);
                    int maxIterationsSinceLastTransition = (int)(parameters.MaxPassesSinceLastTransition * neighborhood.Power);
                    int iterationsForTemperatureValue = 0;
                    int iterationsSinceLastTransition = 0;
                    while (iterationsSinceLastTransition < maxIterationsSinceLastTransition)
                    {
                        iteration++;
                        globalIteration++;
                        iterationsForTemperatureValue++;
                        iterationsSinceLastTransition++;
                        ISolution randomNeighbour = neighborhood.GetRandom();
                        double costDifference = randomNeighbour.CostValue - currentSolution.CostValue;
                        if (costDifference < 0 || (costDifference > 0 && (1 - random.NextDouble()) < Math.Exp(-costDifference / temperature)))
                        {
                            iterationsSinceLastTransition = 0;
                            currentSolution = randomNeighbour;
                            currentSolution.IterationNumber = iteration;
                            currentSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                            currentSolution.IsCurrentBest = false;
                            currentSolution.IsFinal = false;
                            if (!parameters.OutputImprovementsOnly) lock (thisLock) solutionsQueue.Enqueue(currentSolution);
                            if (currentSolution.CostValue < bestSolution.CostValue)
                            {
                                bestSolution = currentSolution;
                                lock (thisLock) bestList.Add(bestSolution);
                            }
                            neighborhood.MoveToSolution(currentSolution);
                        }
                        if (iterationsForTemperatureValue >= maxIterationsForTemperatureValue)
                        {
                            iterationsForTemperatureValue = 0;
                            temperature *= parameters.TemperatureCooling;
                            Console.WriteLine("\tSA {0} cost {1} and temperature {2} at iteration {3}, {4}, {5}", i, currentSolution.CostValue, temperature, iteration, maxIterationsSinceLastTransition - iterationsSinceLastTransition, iterationsSinceLastTransition);
                        }
                    }
                    Console.WriteLine("\tSA {0} finished with cost {1} and temperature {2} at iteration {3}", i, currentSolution.CostValue, temperature, iteration);
                    if (parameters.CompleteWithLocalDescent)
                    {
                        LocalDescent<ISolution> localDescent = new LocalDescent<ISolution>(this.operations);
                        bestSolution = localDescent.Minimize(bestSolution, new LocalDescentParameters()
                        {
                            Multistart = 1,
                            IsSteepestDescent = false,
                            OutputImprovementsOnly = true
                        }).Last();
                        bestSolution.IsFinal = false;
                        lock (thisLock) bestList.Add(bestSolution);
                    }
                });
            });

            task.Start();

            while (!task.IsCompleted)
            {
                Thread.Sleep(parameters.OutputDelayInMilliseconds);
                lock (thisLock)
                {
                    if (solutionsQueue.Count > 0)
                    {
                        yield return (T)solutionsQueue.Dequeue();
                        solutionsQueue.Clear();
                    }
                    if (bestList.Count > 0)
                    {
                        ISolution newBestSolution = bestList.OrderBy(x => x.CostValue).First();
                        if (newBestSolution.CostValue < globalBestSolution.CostValue)
                        {
                            globalBestSolution = newBestSolution;
                            globalBestSolution.IsCurrentBest = true;
                            yield return (T)globalBestSolution;
                        }
                        bestList.Clear();
                    }
                }
            }

            globalBestSolution.IterationNumber = globalIteration;
            globalBestSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
            globalBestSolution.IsFinal = true;
            yield return (T)globalBestSolution;
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