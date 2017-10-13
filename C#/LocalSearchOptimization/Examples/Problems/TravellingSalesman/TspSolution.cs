using System;
using System.Collections.Generic;
using System.Linq;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples.Structures.Permutation;
using System.Text;

namespace LocalSearchOptimization.Examples.Problems.TravellingSalesman
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

        public string InstanceTag { get; set; }

        public string OperatorTag { get; }

        public double[] X { get; }
        public double[] Y { get; }

        public double MaxWidth { get; set; }
        public double MaxHeight { get; set; }

        public int NumberOfCities { get => tspProblem.Dimension; }

        public double LowerBoundGap { get => (CostValue / tspProblem.LowerBound - 1) * 100; }

        public TspSolution(TspProblem tspProblem) : this(tspProblem, Enumerable.Range(0, tspProblem.Dimension).ToList(), "init")
        {

        }

        private TspSolution(TspProblem tspProblem, List<int> order, string operatorName)
        {
            this.tspProblem = tspProblem;
            X = tspProblem.X;
            Y = tspProblem.Y;
            Order = order;
            OperatorTag = operatorName;
            DecodeSolution(tspProblem);
        }

        public IPermutation FetchPermutation(List<int> order, string operatorName)
        {
            return new TspSolution(tspProblem, order, operatorName);
        }

        public ISolution Shuffle(int seed)
        {
            Random random = new Random(seed);
            return new TspSolution(tspProblem, Order.OrderBy(x => random.Next()).ToList(), "shuffle");
        }

        public ISolution Transcode()
        {
            return this;
        }

        private void DecodeSolution(TspProblem problem)
        {
            int city = Order[problem.Dimension - 1];
            int nextCity = Order[0];
            double cost = problem.Distance[city, nextCity];
            MaxWidth = problem.X[city];
            MaxHeight = problem.Y[city];
            for (int i = 0; i < problem.Dimension - 1; i++)
            {
                city = Order[i];
                nextCity = Order[i + 1];
                cost += problem.Distance[city, nextCity];
                if (MaxWidth < problem.X[i]) MaxWidth = problem.X[i];
                if (MaxHeight < problem.Y[i]) MaxHeight = problem.Y[i];
            }
            CostValue = cost;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (int i in Order)
            {
                builder.Append(i).Append("->");
            }
            return builder.ToString();
        }
    }
}