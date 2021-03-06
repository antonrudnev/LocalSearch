﻿using System.Collections.Generic;

namespace LocalSearchOptimization.Components
{
    public abstract class Operator
    {
        public double Weight { get; set; }

        public List<Configuration> Configurations { get; }

        public int Power
        {
            get
            {
                return Configurations.Count;
            }
        }

        public Operator(double weight)
        {
            Weight = weight;
            Configurations = new List<Configuration>();
        }

        public abstract ISolution Apply(ISolution solution, Configuration configuration);

        public ISolution Apply(ISolution solution, int configuration)
        {
            return Apply(solution, Configurations[configuration]);
        }
    }
}