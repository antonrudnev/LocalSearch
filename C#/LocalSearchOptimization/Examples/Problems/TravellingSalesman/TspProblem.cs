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
            X = new double[numberOfCities];
            Y = new double[numberOfCities];
            Distance = new double[numberOfCities, numberOfCities];

            Random random = new Random(seed);

            for (int i = 0; i < numberOfCities; i++)
            {
                X[i] = random.NextDouble();
                Y[i] = random.NextDouble();
            }

            for (int i = 0; i < numberOfCities - 1; i++)
                for (int j = i + 1; j < numberOfCities; j++)
                {
                    Distance[i, j] = Math.Sqrt(Math.Pow(X[i] - X[j], 2) + Math.Pow(Y[i] - Y[j], 2));
                    Distance[j, i] = Distance[i, j];
                }

            double lb1 = 0.7080 * Math.Sqrt(Dimension) + 0.522;
            double lb2 = 0.7078 * Math.Sqrt(Dimension) + 0.551;
            LowerBound = Math.Max(lb1, lb2);
        }
    }
}