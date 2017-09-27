using System;
using System.Collections.Generic;
using System.Linq;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples.Structures.Permutation;
using LocalSearchOptimization.Examples.Structures.Tree;

namespace LocalSearchOptimization.Examples.RectangularPacking
{
    public class FloorplanSolution : IPermutation, ITreeStructure
    {
        private FloorplanProblem floorplanProblem;

        public double CostValue { get; private set; }
        public int IterationNumber { get; set; }
        public double TimeInSeconds { get; set; }
        public bool IsCurrentBest { get; set; }
        public bool IsFinal { get; set; }

        public List<int> Order { get; }

        public List<bool> Branching { get; }

        public string OperatorTag { get; }

        public string InstanceTag { get; set; }

        public List<Tuple<string, int, double>> SolutionsHistory { get; set; }

        public double[] X { get; }
        public double[] Y { get; }
        public double[] W { get; }
        public double[] H { get; }

        public double MaxWidth { get; set; }
        public double MaxHeight { get; set; }

        public int NumberOfRectangles { get => floorplanProblem.Dimension; }

        public FloorplanSolution(FloorplanProblem floorplanProblem) : this(floorplanProblem, Enumerable.Range(1, floorplanProblem.Dimension).ToList(), Enumerable.Repeat(false, floorplanProblem.Dimension).ToList().Concat(Enumerable.Repeat(true, floorplanProblem.Dimension)).ToList(), "init")
        {

        }

        private FloorplanSolution(FloorplanProblem floorplanProblem, List<int> permutation, List<bool> branching, string operationName)
        {
            this.floorplanProblem = floorplanProblem;
            this.X = new double[floorplanProblem.Dimension + 1];
            this.Y = new double[floorplanProblem.Dimension + 1];
            this.W = floorplanProblem.W;
            this.H = floorplanProblem.H;
            Order = permutation;
            Branching = branching;
            OperatorTag = operationName;
            DecodeSolution(floorplanProblem);
        }

        public IPermutation FetchPermutation(List<int> permutation, string operationName)
        {
            return new FloorplanSolution(this.floorplanProblem, permutation, Branching, operationName);
        }

        public ITreeStructure FetchBranching(List<bool> branching, string operationName)
        {
            return new FloorplanSolution(this.floorplanProblem, Order, branching, operationName);
        }

        public ISolution Shuffle(int seed)
        {
            Random random = new Random(seed);

            List<bool> branching = new List<bool>();
            int opened = 0;
            int completed = 0;
            for (int i = 0; i < 2 * this.floorplanProblem.Dimension; i++)
            {
                if (completed < this.floorplanProblem.Dimension && (opened == 0 || random.NextDouble() < 0.5))
                {
                    branching.Add(false);
                    opened++;
                    completed++;
                }
                else
                {
                    branching.Add(true);
                    opened--;
                }
            }

            return new FloorplanSolution(this.floorplanProblem, Order.OrderBy(x => random.Next()).ToList(), branching, "shuffle");
        }

        public void DecodeSolution(FloorplanProblem problem)
        {
            MaxWidth = 0;
            MaxHeight = 0;
            X[0] = 0;
            Y[0] = 0;
            List<int> contour = new List<int> { 0 };
            int currentBlock;
            int perm = 0;
            foreach (bool b in Branching)
            {
                if (!b)
                {
                    int currentContour = contour[0];
                    currentBlock = Order[perm];
                    X[currentBlock] = X[currentContour] + W[currentContour];
                    double maxY = 0;
                    for (int i = 0; i < perm; i++)
                    {
                        int p = Order[i];
                        if ((X[p] < X[currentBlock] + W[currentBlock]) && (X[currentBlock] < X[p] + W[p]) && (maxY < Y[p] + H[p]))
                        {
                            maxY = Y[p] + H[p];
                        }
                    }
                    Y[currentBlock] = maxY;
                    perm++;
                    contour.Insert(0, currentBlock);
                    double currentW = X[currentBlock] + W[currentBlock];
                    double currentH = Y[currentBlock] + H[currentBlock];
                    if (MaxWidth < currentW)
                    {
                        MaxWidth = currentW;
                    }
                    if (MaxHeight < currentH)
                    {
                        MaxHeight = currentH;
                    }
                }
                else
                {
                    contour.RemoveAt(0);
                }
            }
            CostValue = Math.Pow(MaxWidth + MaxHeight, 2);
        }

        //public override string ToString()
        //{
        //    StringBuilder s = new StringBuilder();
        //    foreach (int i in Order)
        //    {
        //        s.Append(i + " ");
        //    }
        //    s.Append("\n");
        //    foreach (bool i in Branching)
        //    {
        //        s.Append(i ? ")" : "(");
        //    }
        //    return s.ToString();
        //}
    }
}