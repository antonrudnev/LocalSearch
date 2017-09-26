using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples.Structures.Permutation;

namespace LocalSearchOptimization.Examples.Problems.TravelingSalesman
{
    public class TspSolution : IPermutation, IGeometricalSolution
    {
        private TspProblem tspProblem;

        public double CostValue { get; private set; }
        public int IterationNumber { get; set; }
        public double TimeInSeconds { get; set; }
        public bool IsCurrentBest { get; set; }
        public bool IsFinal { get; set; }

        public List<int> Order { get; }

        public string OperatorTag { get; }

        public string InstanceTag { get; set; }

        public List<ISolution> SolutionsHistory { get; set; } 

        public ProblemGeometry Details { get; private set; }

        public TspSolution(TspProblem tspProblem) : this(tspProblem, Enumerable.Range(0, tspProblem.Dimension).ToList(), "init")
        {

        }

        private TspSolution(TspProblem tspProblem, List<int> permutation, string operationName)
        {
            this.tspProblem = tspProblem;
            Order = permutation;
            OperatorTag = operationName;
            DecodeSolution(tspProblem);
        }

        public IPermutation FetchPermutation(List<int> permutation, string operationName)
        {
            return new TspSolution(this.tspProblem, permutation, operationName);
        }

        public ISolution Shuffle(int seed)
        {
            Random random = new Random(seed);
            return new TspSolution(this.tspProblem, Order.OrderBy(x => random.Next()).ToList(), "shuffle");
        }

        private void DecodeSolution(TspProblem problem)
        {
            int city = Order[problem.Dimension - 1];
            int nextCity = Order[0];
            double cost = problem.Distance[city, nextCity];

            var points = new List<Tuple<double, double>>{
                new Tuple<double, double>(
                    problem.X[city],
                    problem.Y[city])};

            for (int i = 0; i < problem.Dimension - 1; i++)
            {
                city = Order[i];
                nextCity = Order[i + 1];
                cost += problem.Distance[city, nextCity];
                points.Add(new Tuple<double, double>(
                    problem.X[city],
                    problem.Y[city]));
            }

            Details = new ProblemGeometry()
            {
                Points = points,
                MaxWidth = 1,
                MaxHeight = 1,
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