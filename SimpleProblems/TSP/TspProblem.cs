using System;


namespace SimpleProblems.TSP
{
    public class TspProblem
    {
        public double[] X { get; }
        public double[] Y { get; }
        public double[,] Distance { get;}

        public int NumberOfCities { get; }

        public TspProblem(int numberOfCities, int seed = 3)
        {
            NumberOfCities = numberOfCities;
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
        }
    }
}