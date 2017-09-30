using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public string InstanceTag { get; set; }

        public string OperatorTag { get; }

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
            LinearDecoder(problem);
        }

        /// <summary>
        /// Decodes the tree structure into a packing layout. Trick with the contour structure provides O(n) performance that is the most efficient.
        /// </summary>
        /// <param name="problem"></param>
        public void LinearDecoder(FloorplanProblem problem)
        {
            MaxWidth = 0;
            MaxHeight = 0;
            X[0] = 0;
            Y[0] = 0;
            LinkedList<int> countour = new LinkedList<int>();
            countour.AddLast(0);
            LinkedListNode<int> currentContour = countour.First;
            int perm = 0;
            foreach (bool b in Branching)
            {
                if (!b)
                {
                    int parent = currentContour.Value;
                    int current = Order[perm];
                    X[current] = X[parent] + W[parent];
                    LinkedListNode<int> currentTop = currentContour;
                    double maxY = 0;
                    bool topMostFound = false;
                    double rightSide = X[current] + W[current];
                    while (currentTop.Next != null && !topMostFound)
                    {
                        currentTop = currentTop.Next;
                        int top = currentTop.Value;
                        if (maxY < Y[top] + H[top]) maxY = Y[top] + H[top];

                        if (X[top] + W[top] <= rightSide)
                        {
                            currentTop = currentTop.Previous;
                            countour.Remove(top);
                        }
                        else topMostFound = true;
                    }
                    Y[current] = maxY;
                    countour.AddAfter(currentContour, current);
                    currentContour = currentContour.Next;
                    perm++;
                    double currentW = X[current] + W[current];
                    double currentH = Y[current] + H[current];
                    if (MaxWidth < currentW)
                        MaxWidth = currentW;
                    if (MaxHeight < currentH)
                        MaxHeight = currentH;
                }
                else
                {
                    currentContour = currentContour.Previous;
                }
            }
            CostValue = Math.Pow(MaxWidth + 1.05 * MaxHeight, 2);
        }

        /// <summary>
        /// Decodes the tree structure into a packing layout. Straightforward implementation provides O(n^2) performance. Used for demonstration purposes only. 
        /// </summary>
        /// <param name="problem"></param>
        public void QuadraticDecoder(FloorplanProblem problem)
        {
            MaxWidth = 0;
            MaxHeight = 0;
            X[0] = 0;
            Y[0] = 0;
            Stack<int> parents = new Stack<int>();
            parents.Push(0);
            int perm = 0;
            foreach (bool b in Branching)
            {
                if (!b)
                {
                    int parent = parents.Peek();
                    int current = Order[perm];
                    X[current] = X[parent] + W[parent];
                    double maxY = 0;
                    for (int i = 0; i < perm; i++)
                    {
                        int p = Order[i];
                        if ((X[p] < X[current] + W[current]) && (X[current] < X[p] + W[p]) && (maxY < Y[p] + H[p]))
                        {
                            maxY = Y[p] + H[p];
                        }
                    }
                    Y[current] = maxY;
                    perm++;
                    parents.Push(current);
                    double currentW = X[current] + W[current];
                    double currentH = Y[current] + H[current];
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
                    parents.Pop();
                }
            }
            CostValue = MaxWidth * MaxHeight;
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