using System;
using LocalSearch.Solver;
using System.Collections.Generic;
using System.Linq;

namespace LocalSearch.TSP
{
    public class TspNeighbourhood : INeighbourhood
    {
        private TspSolution tspSolution;
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
                return this.tspSolution;
            }
        }

        public TspNeighbourhood(TspProblem tspProblem)
        {
            this.tspSolution = new TspSolution(tspProblem);
        }

        public ISolution GetNext()
        {
            TspSolution neighbourSolution;
            var neighbour = this.neighbours[this.currentNeighbour];

            switch (neighbour.Item3)
            {
                case 0:
                    neighbourSolution = this.tspSolution.Swap(neighbour.Item1, neighbour.Item2);
                    break;
                case 1:
                    neighbourSolution = this.tspSolution.Shift(neighbour.Item1, neighbour.Item2);
                    break;
                default:
                    neighbourSolution = this.tspSolution.Reverse(neighbour.Item1, neighbour.Item2);
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
            int i = random.Next(this.tspSolution.NumberOfCities);
            int j = random.Next(this.tspSolution.NumberOfCities);
            double p = random.NextDouble();
            if (p < 0.4)
                return this.tspSolution.Swap(i, j);
            else if (p < 0.6)
                return this.tspSolution.Shift(i, j);
            else
                return this.tspSolution.Reverse(i, j);
        }

        public void Reset()
        {
            this.neighbours = new List<Tuple<int, int, byte>>();
            for (int i = 0; i < this.tspSolution.NumberOfCities - 1; i++)
            {
                for (int j = i+1; j < this.tspSolution.NumberOfCities; j++)
                {
                    this.neighbours.Add(new Tuple<int, int, byte>(i, j, 0));
                    this.neighbours.Add(new Tuple<int, int, byte>(i, j, 1));
                    this.neighbours.Add(new Tuple<int, int, byte>(i, j, 2));
                }
            }
            this.neighbours = this.neighbours.OrderBy(x => this.random.Next()).ToList();
            this.currentNeighbour = 0;
            this.isScanned = false;
        }

        public void InitToRandom()
        {
            this.tspSolution.InitRandom();
            this.Reset();
        }

        public void MoveToSolution(ISolution solution)
        {
            this.tspSolution = (TspSolution)solution;
            this.currentNeighbour = 0;
            this.isScanned = false;
        }

        public double initProba { get { return 0.001; } }

        public double tempDecrease { get { return 0.9; } }

        public int tempLevelIterations { get { return Math.Min(this.Power * 5, 150000); } }

        public int frozenRate { get { return (int)(tempLevelIterations * 0.05); } }

        public int isLocalOptimaIterations { get { return tempLevelIterations * 3; } }
    }
}