using System;
using System.IO;

namespace LocalSearchOptimization.Examples.RectangularPacking
{
    public class FloorplanProblem
    {
        public double[] W { get; }
        public double[] H { get; }

        public double TotalArea { get; private set; }

        public int Dimension { get; }

        public FloorplanProblem(int numberOfRectangles, int seed = 0)
        {
            Dimension = numberOfRectangles;
            W = new double[numberOfRectangles + 1];
            H = new double[numberOfRectangles + 1];
            Random random = new Random(seed);
            for (int i = 1; i <= numberOfRectangles; i++)
            {
                W[i] = random.NextDouble() + 0.1;
                H[i] = random.NextDouble() + 0.1;
            }
            CalculateMetrics();
        }

        public FloorplanProblem(double[] w, double[] h)
        {
            if (w.Length != h.Length) throw new ArgumentException("Arrays w and h have different lengths.");
            Dimension = w.Length - 1;
            W = w;
            H = h;
            CalculateMetrics();
        }

        private void CalculateMetrics()
        {
            W[0] = 0;
            H[0] = 0;
            TotalArea = 0;
            for (int i = 1; i <= Dimension; i++)
                TotalArea += W[i] * H[i];
        }

        public void Save(string fileName)
        {
            using (StreamWriter file = new StreamWriter(File.Create(fileName)))
            {
                file.WriteLine(Dimension);
                for (int i = 1; i <= Dimension; i++)
                {
                    file.WriteLine("{0} {1}", W[i], H[i]);
                }
            }
        }

        public static FloorplanProblem Load(string fileName)
        {
            using (StreamReader file = new StreamReader(File.Open(fileName, FileMode.Open)))
            {
                int dimension = int.Parse(file.ReadLine());
                double[] w = new double[dimension + 1];
                double[] h = new double[dimension + 1];
                for (int i = 1; i <= dimension; i++)
                {
                    string[] wh = file.ReadLine().Split(' ');
                    w[i] = double.Parse(wh[0]);
                    h[i] = double.Parse(wh[1]);
                }
                return new FloorplanProblem(w, h);
            }
        }
    }
}