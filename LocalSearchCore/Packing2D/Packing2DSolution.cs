using System;
using System.Linq;
using LocalSearch.Solver;
using System.Collections.Generic;

namespace LocalSearch.Packing2D
{
    public class Packing2DSolution : ISolution
    {
        private Packing2DProblem packing2DProblem;
        private List<int> permutation;
        private List<int> tree;
        private Random random = new Random();

        public string NTag { get; set; }

        public Packing2DSolution(Packing2DProblem packing2DProblem)
        {
            this.packing2DProblem = packing2DProblem;
            this.permutation = Enumerable.Range(1, packing2DProblem.NumberOfShapes).ToList();
            this.tree = Enumerable.Repeat(0, packing2DProblem.NumberOfShapes).ToList();
            this.tree.AddRange(Enumerable.Repeat(1, packing2DProblem.NumberOfShapes).ToList());

        }

        public void InitRandom()
        {
            this.permutation = permutation.OrderBy(x => this.random.Next()).ToList();
            this.tree.Clear();
            int branched = 0;
            int branches = 0;
            for (int i = 0; i < 2 * NumberOfShapes; i++)
            {
                if (branches < this.NumberOfShapes && (branched == 0 || random.NextDouble() < 0.5))
                {
                    this.tree.Add(0);
                    branched++;
                    branches++;
                }
                else
                {
                    this.tree.Add(1);
                    branched--;
                }
            }
        }

        private Packing2DSolution(Packing2DProblem packing2DProblem, List<int> permutation, List<int> tree, string n)
        {
            this.packing2DProblem = packing2DProblem;
            this.permutation = permutation;
            this.tree = tree;
            this.NTag = n;
        }

        public int NumberOfShapes
        {
            get
            {
                return this.packing2DProblem.NumberOfShapes;
            }
        }

        public Packing2DSolution Swap(int m, int n)
        {
            List<int> swapped = new List<int>(this.permutation)
            {
                [m] = this.permutation[n],
                [n] = this.permutation[m]
            };
            return new Packing2DSolution(this.packing2DProblem, swapped, this.tree, "swap");
        }

        public Packing2DSolution Shift(int m, int n)
        {
            List<int> swapped = new List<int>(this.permutation);
            int cityToShift = swapped[m];
            swapped.RemoveAt(m);
            if (n <= m)
                swapped.Insert(n, cityToShift);
            else
                swapped.Insert(n - 1, cityToShift);
            return new Packing2DSolution(this.packing2DProblem, swapped, this.tree, "shift");
        }

        public Packing2DSolution Leaf(int m, int n)
        {
            List<int> newTree = new List<int>(this.tree);
            int ind = m;
            int count = 1;
            if (newTree[m] == 0)
            {
                while (count != 0)
                {
                    ind++;
                    count += newTree[ind] == 0 ? 1 : -1;
                }
                newTree.RemoveAt(ind);
                newTree.RemoveAt(m);
            }
            else
            {
                while (count != 0)
                {
                    ind--;
                    count += newTree[ind] == 1 ? 1 : -1;
                }
                newTree.RemoveAt(m);
                newTree.RemoveAt(ind);
            }
            newTree.Insert(n, 1);
            newTree.Insert(n, 0);
            return new Packing2DSolution(this.packing2DProblem, this.permutation, newTree, "leaf");
        }

        public SolutionDetails GetDetails()
        {
            List<Tuple<double, double, double, double>> rectangles = new List<Tuple<double, double, double, double>>();
            double maxW = 0;
            double maxH = 0;
            double[] x = new double[NumberOfShapes + 1];
            double[] y = new double[NumberOfShapes + 1];
            double[] w = this.packing2DProblem.W;
            double[] h = this.packing2DProblem.H;
            x[0] = 0;
            y[0] = 0;
            List<int> contour = new List<int> { 0 };
            int currentBlock;
            int perm = 0;
            foreach (int b in this.tree)
            {
                if (b == 0)
                {
                    int currentContour = contour[0];
                    currentBlock = permutation[perm];
                    x[currentBlock] = x[currentContour] + w[currentContour];
                    double maxY = 0;
                    for (int i = 0; i < perm; i++)
                    {
                        int p = permutation[i];
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

            return new SolutionDetails()
            {
                CostValue = maxW * maxH+ maxH,
                Rectangles = rectangles,
                MaxW = maxW,
                MaxH = maxH,
                N = NTag
            };
        }
    }
}