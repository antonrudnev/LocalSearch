using System;

namespace LocalSearchOptimization.Examples.RectangularPacking
{
    public class FloorplanProblem
    {
        public double[] W { get; }
        public double[] H { get; }

        public int Dimension { get; }

        public FloorplanProblem(int numberOfRectangles, int seed = 0)
        {
            Dimension = numberOfRectangles;
            this.W = new double[numberOfRectangles + 1];
            this.H = new double[numberOfRectangles + 1];

            Random random = new Random(seed);

            this.W[0] = 0;
            this.H[0] = 0;

            for (int i = 1; i <= numberOfRectangles; i++)
            {
                this.W[i] = random.NextDouble() + 0.1;
                this.H[i] = random.NextDouble() + 0.1;
            }
        }
    }
}