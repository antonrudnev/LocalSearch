using System;

namespace LocalSearch.Packing2D
{
    public class Packing2DProblem
    {
        public double[] W { get; }
        public double[] H { get; }

        public int NumberOfShapes { get; }

        public Packing2DProblem(int numberOfShapes)
        {
            NumberOfShapes = numberOfShapes;
            this.W = new double[numberOfShapes+1];
            this.H = new double[numberOfShapes+1];

            Random random = new Random(0);

            this.W[0] = 0;
            this.H[0] = 0;

            for (int i = 1; i <= numberOfShapes; i++)
            {
                this.W[i] = random.NextDouble() + 0.1;
                this.H[i] = random.NextDouble() + 0.1;
            }
        }
    }
}
