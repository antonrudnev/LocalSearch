﻿using System;
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

        public string DerivedByOperation { get; }

        public ProblemGeometry Details { get; private set; }

        public TspSolution(TspProblem tspProblem) : this(tspProblem, Enumerable.Range(0, tspProblem.Dimension).ToList(), "init")
        {

        }

        private TspSolution(TspProblem tspProblem, List<int> permutation, string operationName)
        {
            this.tspProblem = tspProblem;
            Order = permutation;
            DerivedByOperation = operationName;
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
            var lines = new List<Tuple<double, double, double, double>>
            {
                new Tuple<double, double, double, double>(
                    problem.X[city],
                    problem.Y[city],
                    problem.X[nextCity],
                    problem.Y[nextCity])
            };

            for (int i = 0; i < problem.Dimension - 1; i++)
            {
                city = Order[i];
                nextCity = Order[i + 1];
                cost += problem.Distance[city, nextCity];
                lines.Add(new Tuple<double, double, double, double>(
                    problem.X[city],
                    problem.Y[city],
                    problem.X[nextCity],
                    problem.Y[nextCity]));
            }

            Details = new ProblemGeometry()
            {
                Lines = lines,
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