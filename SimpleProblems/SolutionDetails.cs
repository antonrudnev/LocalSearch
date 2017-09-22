using System;
using System.Collections.Generic;

namespace LocalSearch.Components
{
    public class SolutionDetails
    {
        public double MaxW { get; set; }

        public double MaxH { get; set; }

        public List<Tuple<double, double, double, double>> Lines { get; set; }

        public List<Tuple<double, double, double, double>> Rectangles { get; set; }

        public SolutionDetails()
        {
            Lines = new List<Tuple<double, double, double, double>>();
            Rectangles = new List<Tuple<double, double, double, double>>();
        }
    }
}