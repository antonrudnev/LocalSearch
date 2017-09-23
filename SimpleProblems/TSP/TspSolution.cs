using System;
using System.Linq;
using System.Collections.Generic;
using LocalSearch.Components;
using System.Text;
using SimpleProblems.Permutations;

namespace SimpleProblems.TSP
{
    public class TspSolution : IPermutation
    {
        private TspProblem tspProblem;

        public double CostValue { get; private set; }
        public int IterationNumber { get; set; }
        public double TimeInSeconds { get; set; }
        public bool IsCurrentBest { get; set; }
        public bool IsFinal { get; set; }

        public List<int> Order { get; }

        public string OperationName { get; }

        public SolutionDetails Details { get; private set; }

        public TspSolution(TspProblem tspProblem) : this(tspProblem, Enumerable.Range(0, tspProblem.NumberOfCities).ToList(), "init")
        {

        }

        public TspSolution(TspProblem tspProblem, List<int> permutation, string operationName)
        {
            this.tspProblem = tspProblem;
            Order = permutation;
            OperationName = operationName;
            DecodeSolution(tspProblem);
        }

        public IPermutation DeriveFromPermutation(List<int> permutation, string operationName)
        {
            return new TspSolution(this.tspProblem, permutation, operationName);
        }

        public ISolution Shuffle(int seed)
        {
            Random random = new Random(seed);
            return new TspSolution(this.tspProblem, Order.OrderBy(x => random.Next()).ToList(), "shuffle");
        }

        private void DecodeSolution(TspProblem tspProblem)
        {
            int city = Order[tspProblem.NumberOfCities - 1];
            int nextCity = Order[0];
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
                city = Order[i];
                nextCity = Order[i + 1];
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

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (int i in Order)
            {
                s.Append(i + " ");
            }
            return s.ToString();
        }
    }
}