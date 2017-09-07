using System;
using LocalSearch.Solver;
using System.Collections.Generic;
using System.Linq;

namespace LocalSearch.Packing2D
{
    public class Packing2DNeighbourhood : INeighbourhood
    {
        private Packing2DSolution packing2DSolution;
        private bool isScanned;
        private int currentNeighbour;
        private List<Tuple<int, int, byte>> neighbours;
        private Random random = new Random();

        public int Power
        {
            get
            {
                return neighbours.Count;
            }
        }

        public bool IsScanned
        {
            get
            {
                return this.isScanned;
            }
        }

        public ISolution CurrentSolution
        {
            get
            {
                return this.packing2DSolution;
            }
        }

        public Packing2DNeighbourhood(Packing2DProblem packing2DProblem)
        {
            this.packing2DSolution = new Packing2DSolution(packing2DProblem);
        }

        public ISolution GetNext()
        {
            Packing2DSolution neighbourSolution;
            var neighbour = this.neighbours[this.currentNeighbour];

            switch (neighbour.Item3)
            {
                case 0:
                    neighbourSolution = this.packing2DSolution.Swap(neighbour.Item1, neighbour.Item2);
                    break;
                case 1:
                    neighbourSolution = this.packing2DSolution.Shift(neighbour.Item1, neighbour.Item2);
                    break;
                default:
                    neighbourSolution = this.packing2DSolution.Leaf(neighbour.Item1, neighbour.Item2);
                    break;
            }
            this.currentNeighbour++;
            if (currentNeighbour >= neighbours.Count)
            {
                this.isScanned = true;
            }

            return neighbourSolution;
        }

        public ISolution GetRandom()
        {
            int n = this.packing2DSolution.NumberOfShapes;
            int i = random.Next(2 * n);
            int j = random.Next(2 * n - 2);
            double p = random.NextDouble();
            if (p < 0.7)
                return this.packing2DSolution.Swap(i % n, j % n);
            else if (p < 0.99)
                return this.packing2DSolution.Leaf(i, j);
            else
                return this.packing2DSolution.Shift(i % n, j % n);
        }

        public void Reset()
        {
            this.neighbours = new List<Tuple<int, int, byte>>();
            for (int i = 0; i < this.packing2DSolution.NumberOfShapes - 1; i++)
            {
                for (int j = i + 1; j < this.packing2DSolution.NumberOfShapes; j++)
                {
                    this.neighbours.Add(new Tuple<int, int, byte>(i, j, 0));
                    this.neighbours.Add(new Tuple<int, int, byte>(i, j, 1));
                }
            }
            for (int i = 0; i < 2 * this.packing2DSolution.NumberOfShapes; i++)
            {
                for (int j = 0; j < 2 * this.packing2DSolution.NumberOfShapes - 2; j++)
                {
                    this.neighbours.Add(new Tuple<int, int, byte>(i, j, 2));
                }
            }
            this.neighbours = this.neighbours.OrderBy(x => this.random.Next()).ToList();
            this.currentNeighbour = 0;
            this.isScanned = false;
        }

        public void InitToRandom()
        {
            this.packing2DSolution.InitRandom();
            this.Reset();
        }

        public void MoveToSolution(ISolution solution)
        {
            this.packing2DSolution = (Packing2DSolution)solution;
            this.currentNeighbour = 0;
            this.isScanned = false;
        }


        public double initProba { get { return 0.001; } }

        public double tempDecrease { get { return 0.95; } }

        public int tempLevelIterations { get { return Math.Min(this.Power * 2, 150000); } }

        public int frozenRate { get { return (int)(tempLevelIterations * 0.05); } }

        public int isLocalOptimaIterations { get { return tempLevelIterations; } }

    }
}