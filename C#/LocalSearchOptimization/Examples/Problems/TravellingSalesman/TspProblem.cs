using System;
using System.IO;
using System.Linq;


namespace LocalSearchOptimization.Examples.Problems.TravellingSalesman
{
    public class TspProblem
    {
        public double[] X { get; }
        public double[] Y { get; }
        public double[,] Distance { get; private set; }

        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }

        public int Dimension { get; }

        public double LowerBound { get; private set; }

        public TspProblem(int numberOfCities, int seed = 3)
        {
            Dimension = numberOfCities;
            X = new double[Dimension];
            Y = new double[Dimension];
            Random random = new Random(seed);
            for (int i = 0; i < Dimension; i++)
            {
                X[i] = random.NextDouble();
                Y[i] = random.NextDouble();
            }
            CalculateMetrics();
        }

        public TspProblem(double[] x, double[] y)
        {
            if (x.Length != y.Length) throw new ArgumentException("Arrays x and y have different lengths.");
            Dimension = x.Length;
            X = x;
            Y = y;
            CalculateMetrics();
        }

        private void CalculateMetrics()
        {
            Distance = new double[Dimension, Dimension];
            for (int i = 0; i < Dimension - 1; i++)
                for (int j = i + 1; j < Dimension; j++)
                {
                    Distance[i, j] = Math.Sqrt(Math.Pow(X[i] - X[j], 2) + Math.Pow(Y[i] - Y[j], 2));
                    Distance[j, i] = Distance[i, j];
                }

            MinX = X.Min();
            MinY = Y.Min();
            MaxX = X.Max();
            MaxY = Y.Max();

            double lb1 = 0.7080 * Math.Sqrt(Dimension) + 0.522;
            double lb2 = 0.7078 * Math.Sqrt(Dimension) + 0.551;
            LowerBound = Math.Max(lb1, lb2);
        }

        public void Save(string fileName)
        {
            using (StreamWriter file = new StreamWriter(File.Create(fileName)))
            {
                file.WriteLine(Dimension);
                for (int i = 0; i < Dimension; i++)
                {
                    file.WriteLine("{0} {1}", X[i], Y[i]);
                }
            }
        }

        public static TspProblem Load(string fileName)
        {
            using (StreamReader file = new StreamReader(File.Open(fileName, FileMode.Open)))
            {
                int dimension = int.Parse(file.ReadLine());
                double[] x = new double[dimension];
                double[] y = new double[dimension];
                for (int i = 0; i < dimension; i++)
                {
                    string[] xy = file.ReadLine().Split(' ');
                    x[i] = double.Parse(xy[0]);
                    y[i] = double.Parse(xy[1]);
                }
                return new TspProblem(x, y);
            }
        }
    }
}