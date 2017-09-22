using System;
using System.Collections.Generic;
using LocalSearch.Components;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace LocalSearch.Solver
{
    public class LocalDescent<T> where T: ISolution
    {
        private List<Operation> operations;

        private Object thisLock = new Object();

        public LocalDescent(List<Operation> operations)
        {
            this.operations = operations;
        }

        public IEnumerable<T> Minimize(T solution, LocalDescentParameters parameters)
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
                    int iteration = 0;
                    ISolution bestSolution = solution;
                    Neighborhood neighborhood = new Neighborhood(i == 0 ? solution : solution.Shuffle(), operations);
                    ISolution subOptimalSolution = neighborhood.CurrentSolution;
                    ISolution bestNeighbor = subOptimalSolution;
                    bool bestFound = false;
                    while (!bestFound)
                    {
                        foreach (ISolution neighbor in neighborhood.Neighbors)
                        {
                            iteration++;
                            globalIteration++;
                            if (neighbor.CostValue < bestNeighbor.CostValue)
                            {
                                bestNeighbor = neighbor;
                                if (!parameters.IsSteepestDescent) break;
                            }
                        }
                        if (bestNeighbor.CostValue < subOptimalSolution.CostValue)
                        {
                            subOptimalSolution = bestNeighbor;
                            subOptimalSolution.IterationNumber = iteration;
                            subOptimalSolution.TimeInSeconds = (DateTime.Now - startedAt).TotalSeconds;
                            subOptimalSolution.IsCurrentBest = false;
                            subOptimalSolution.IsFinal = false;
                            if (!parameters.OutputImprovementsOnly) lock (thisLock) solutionsQueue.Enqueue(subOptimalSolution);
                            if (subOptimalSolution.CostValue < bestSolution.CostValue)
                            {
                                bestSolution = subOptimalSolution;
                                lock (thisLock) bestList.Add(bestSolution);
                            }
                            neighborhood.MoveToSolution(subOptimalSolution);
                        }
                        else
                        {
                            bestFound = true;
                        }
                    }
                    Console.WriteLine("\tLD {0} finished with cost {1} at iteration {2}", parameters.Multistart > 1 ? new int?(i) : null, subOptimalSolution.CostValue, iteration);
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
    }
}