using System;
using System.Collections.Generic;

namespace LocalSearchOptimization.Examples
{
    public class ProblemGeometry
    {
        public double MaxWidth { get; set; }

        public double MaxHeight { get; set; }

        public List<Tuple<double, double>> Points { get; set; }

        public List<Tuple<double, double, double, double>> Rectangles { get; set; }
    }
}