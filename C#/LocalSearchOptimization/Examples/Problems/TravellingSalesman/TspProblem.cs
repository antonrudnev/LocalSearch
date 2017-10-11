using System;

namespace LocalSearchOptimization.Examples.Problems.TravellingSalesman
{
    public class TspProblem
    {
        public double[] X { get; }
        public double[] Y { get; }
        public double[,] Distance { get; }

        public int Dimension { get; }

        public double LowerBound { get; }

        public TspProblem(int numberOfCities, int seed = 3)
        {
            Dimension = numberOfCities;
            this.X = new double[numberOfCities];
            this.Y = new double[numberOfCities];
            this.Distance = new double[numberOfCities, numberOfCities];

            Random random = new Random(seed);

            for (int i = 0; i < numberOfCities; i++)
            {
                this.X[i] = random.NextDouble();
                this.Y[i] = random.NextDouble();
            }

            for (int i = 0; i < numberOfCities - 1; i++)
                for (int j = i + 1; j < numberOfCities; j++)
                {
                    this.Distance[i, j] = Math.Sqrt(Math.Pow(this.X[i] - this.X[j], 2) + Math.Pow(this.Y[i] - this.Y[j], 2));
                    this.Distance[j, i] = this.Distance[i, j];
                }

            double lb1 = 0.7080 * Math.Sqrt(Dimension) + 0.522;
            double lb2 = 0.7078 * Math.Sqrt(Dimension) + 0.551;
            LowerBound = Math.Max(lb1, lb2);
        }
    }
}