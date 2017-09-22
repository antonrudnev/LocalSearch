using System;
using System.Collections.Generic;

namespace LocalSearch.Solver
{
    public class SolutionDetails
    {
        public double CostValue { get; set; }

        public double MaxW { get; set; }

        public double MaxH { get; set; }

        public List<Tuple<double, double, double, double>> Lines { get; set; }

        public List<Tuple<double, double, double, double>> Rectangles { get; set; }

        public string N { get; set; }

        public int Iteration { get; set; }

        public double TimeInSeconds { get; set; }

        public bool IsBest { get; set; }

        public SolutionDetails()
        {
            Lines = new List<Tuple<double, double, double, double>>();
            Rectangles = new List<Tuple<double, double, double, double>>();
        }
    }
}