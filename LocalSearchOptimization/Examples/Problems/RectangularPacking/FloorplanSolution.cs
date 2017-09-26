using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples.Structures.Permutation;
using LocalSearchOptimization.Examples.Structures.Tree;
using LocalSearchOptimization.Examples.Problems;

namespace LocalSearchOptimization.Examples.RectangularPacking
{
    public class FloorplanSolution : IPermutation, ITreeStructure, IGeometricalSolution
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

        public List<ISolution> SolutionsHistory { get; set; }

        public ProblemGeometry Details { get; private set; }

        public FloorplanSolution(FloorplanProblem floorplanProblem) : this(floorplanProblem, Enumerable.Range(1, floorplanProblem.Dimension).ToList(), Enumerable.Repeat(false, floorplanProblem.Dimension).ToList().Concat(Enumerable.Repeat(true, floorplanProblem.Dimension)).ToList(), "init")
        {

        }

        private FloorplanSolution(FloorplanProblem floorplanProblem, List<int> permutation, List<bool> branching, string operationName)
        {
            this.floorplanProblem = floorplanProblem;
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

        //public FloorplanSolution Leaf(int m, int n)
        //{
        //    List<int> newTree = new List<int>(this.tree);
        //    int ind = m;
        //    int count = 1;
        //    if (newTree[m] == 0)
        //    {
        //        while (count != 0)
        //        {
        //            ind++;
        //            count += newTree[ind] == 0 ? 1 : -1;
        //        }
        //        newTree.RemoveAt(ind);
        //        newTree.RemoveAt(m);
        //    }
        //    else
        //    {
        //        while (count != 0)
        //        {
        //            ind--;
        //            count += newTree[ind] == 1 ? 1 : -1;
        //        }
        //        newTree.RemoveAt(m);
        //        newTree.RemoveAt(ind);
        //    }
        //    newTree.Insert(n, 1);
        //    newTree.Insert(n, 0);
        //    return new FloorplanSolution(this.floorplanProblem, this.permutation, newTree, "leaf");
        //}

        public void DecodeSolution(FloorplanProblem problem)
        {
            List<Tuple<double, double, double, double>> rectangles = new List<Tuple<double, double, double, double>>();
            double maxW = 0;
            double maxH = 0;
            double[] x = new double[problem.Dimension + 1];
            double[] y = new double[problem.Dimension + 1];
            double[] w = problem.W;
            double[] h = problem.H;
            x[0] = 0;
            y[0] = 0;
            List<int> contour = new List<int> { 0 };
            int currentBlock;
            int perm = 0;
            foreach (bool b in Branching)
            {
                if (!b)
                {
                    int currentContour = contour[0];
                    currentBlock = Order[perm];
                    x[currentBlock] = x[currentContour] + w[currentContour];
                    double maxY = 0;
                    for (int i = 0; i < perm; i++)
                    {
                        int p = Order[i];
                        if ((x[p] < x[currentBlock] + w[currentBlock]) && (x[currentBlock] < x[p] + w[p]) && (maxY < y[p] + h[p]))
                        {
                            maxY = y[p] + h[p];
                        }
                    }
                    y[currentBlock] = maxY;
                    perm++;
                    contour.Insert(0, currentBlock);

                    rectangles.Add(new Tuple<double, double, double, double>(x[currentBlock], y[currentBlock], w[currentBlock], h[currentBlock]));
                    double currentW = x[currentBlock] + w[currentBlock];
                    double currentH = y[currentBlock] + h[currentBlock];
                    if (maxW < currentW)
                    {
                        maxW = currentW;
                    }
                    if (maxH < currentH)
                    {
                        maxH = currentH;
                    }
                }
                else
                {
                    contour.RemoveAt(0);
                }
            }

            double maxSize = Math.Max(maxW, maxH);

            Details = new ProblemGeometry()
            {
                Rectangles = rectangles,
                MaxWidth = maxSize,
                MaxHeight = maxSize
            };

            CostValue = Math.Pow(maxW + maxH,2);
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (int i in Order)
            {
                s.Append(i + " ");
            }
            s.Append("\n");
            foreach (bool i in Branching)
            {
                s.Append(i ? ")" : "(");
            }
            return s.ToString();
        }
    }
}