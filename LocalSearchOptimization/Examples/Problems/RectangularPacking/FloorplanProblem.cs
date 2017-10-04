using System;

namespace LocalSearchOptimization.Examples.RectangularPacking
{
    public class FloorplanProblem
    {
        private double totalArea;

        public double[] W { get; }
        public double[] H { get; }

        public double TotalArea { get => totalArea; }

        public int Dimension { get; }

        public FloorplanProblem(int numberOfRectangles, int seed = 0)
        {
            Dimension = numberOfRectangles;
            W = new double[numberOfRectangles + 1];
            H = new double[numberOfRectangles + 1];

            Random random = new Random(seed);

            W[0] = 0;
            H[0] = 0;
            totalArea = 0;
            for (int i = 1; i <= numberOfRectangles; i++)
            {
                W[i] = random.NextDouble() + 0.1;
                H[i] = random.NextDouble() + 0.1;
                totalArea += W[i] * H[i];
            }
        }
    }
}