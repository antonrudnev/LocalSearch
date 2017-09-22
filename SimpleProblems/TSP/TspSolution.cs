using System;
using System.Linq;
using System.Collections.Generic;
using LocalSearch.Components;

namespace LocalSearch.TSP
{
    public class TspSolution : IPermutation
    {
        private Random random = new Random();
        private TspProblem tspProblem;

        public double CostValue { get; private set; }
        public int IterationNumber { get; set; }
        public double TimeInSeconds { get; set; }
        public bool IsCurrentBest { get; set; }
        public bool IsFinal { get; set; }

        public List<int> Permutation { get; }

        public string OperationName { get; }

        public SolutionDetails Details { get; private set; }

        public TspSolution(TspProblem tspProblem) : this(tspProblem, Enumerable.Range(0, tspProblem.NumberOfCities).ToList(), "init")
        {

        }

        public TspSolution(TspProblem tspProblem, List<int> permutation, string operationName)
        {
            this.tspProblem = tspProblem;
            Permutation = permutation;
            OperationName = operationName;
            DecodeSolution(tspProblem);
        }     

        public IPermutation DeriveFromPermutation(List<int> permutation, string operationName)
        {
            return new TspSolution(this.tspProblem, permutation, operationName);
        }

        public ISolution Shuffle()
        {
            return new TspSolution(this.tspProblem, Permutation.OrderBy(x => this.random.Next()).ToList(), "shuffle");
        }

        private void DecodeSolution(TspProblem tspProblem)
        {
            int city = Permutation[tspProblem.NumberOfCities - 1];
            int nextCity = Permutation[0];
            double cost = tspProblem.Distance[city, nextCity];
            var lines = new List<Tuple<double, double, double, double>>
            {
                new Tuple<double, double, double, double>(
                    tspProblem.X[city],
                    tspProblem.Y[city],
                    tspProblem.X[nextCity],
                    tspProblem.Y[nextCity])
            };

            for (int i = 0; i < tspProblem.NumberOfCities - 1; i++)
            {
                city = Permutation[i];
                nextCity = Permutation[i + 1];
                cost += tspProblem.Distance[city, nextCity];
                lines.Add(new Tuple<double, double, double, double>(
                    tspProblem.X[city],
                    tspProblem.Y[city],
                    tspProblem.X[nextCity],
                    tspProblem.Y[nextCity]));
            }

            Details = new SolutionDetails()
            {
                Lines = lines,
                MaxW = 1,
                MaxH = 1,
            };

            CostValue = cost;
        }
    }
}