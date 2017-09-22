using System;
using System.Linq;
using LocalSearch.Solver;
using System.Collections.Generic;

namespace LocalSearch.TSP
{
    public class TspSolution : ISolution
    {
        private TspProblem tspProblem;
        private List<int> permutation;
        private Random random = new Random();

        public string NTag { get; set; }

        public TspSolution(TspProblem tspProblem)
        {
            this.tspProblem = tspProblem;
            this.permutation = Enumerable.Range(0, tspProblem.NumberOfCities).ToList();
        }

        public void InitRandom()
        {
            this.permutation = permutation.OrderBy(x => this.random.Next()).ToList();
        }

        private TspSolution(TspProblem tspProblem, List<int> permutation, string n)
        {
            this.tspProblem = tspProblem;
            this.permutation = permutation;
            this.NTag = n;
        }

        public int NumberOfCities
        {
            get
            {
                return this.tspProblem.NumberOfCities;
            }
        }

        public TspSolution Swap(int m, int n)
        {
            List<int> swapped = new List<int>(this.permutation)
            {
                [m] = this.permutation[n],
                [n] = this.permutation[m]
            };
            return new TspSolution(this.tspProblem, swapped, "swap");
        }

        public TspSolution Shift(int m, int n)
        {
            List<int> swapped = new List<int>(this.permutation);
            int cityToShift = swapped[m];
            swapped.RemoveAt(m);
            if (n <= m)
                swapped.Insert(n, cityToShift);
            else
                swapped.Insert(n - 1, cityToShift);
            return new TspSolution(this.tspProblem, swapped, "shift");
        }

        public TspSolution Reverse(int m, int n)
        {
            int i = Math.Min(m, n);
            int j = Math.Max(m, n);
            List<int> swapped = new List<int>(this.permutation);
            swapped.Reverse(i, j - i + 1);
            return new TspSolution(this.tspProblem, swapped, "2opt");
        }

        public SolutionDetails GetDetails()
        {
            int city = this.permutation[this.tspProblem.NumberOfCities - 1];
            int nextCity = this.permutation[0];
            double cost = this.tspProblem.Distance[city, nextCity];
            var lines = new List<Tuple<double, double, double, double>>
            {
                new Tuple<double, double, double, double>(
                    this.tspProblem.X[city],
                    this.tspProblem.Y[city],
                    this.tspProblem.X[nextCity],
                    this.tspProblem.Y[nextCity])
            };

            for (int i = 0; i < this.tspProblem.NumberOfCities - 1; i++)
            {
                city = this.permutation[i];
                nextCity = this.permutation[i + 1];
                cost += this.tspProblem.Distance[city, nextCity];
                lines.Add(new Tuple<double, double, double, double>(
                    this.tspProblem.X[city],
                    this.tspProblem.Y[city],
                    this.tspProblem.X[nextCity],
                    this.tspProblem.Y[nextCity]));
            }

            return new SolutionDetails()
            {
                CostValue = cost,
                Lines = lines,
                MaxW = 1,
                MaxH = 1,
                N = NTag
            };
        }
    }
}