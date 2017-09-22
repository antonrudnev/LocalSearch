using System;
using System.Collections.Generic;

namespace LocalSearch.Components
{
    public abstract class Operation
    {
        private Random random = new Random();

        public double Weight { get; set; }

        public List<Configuration> Configurations { get; }

        public Operation(int dimension, double weight = 1)
        {
            Weight = weight;
            Configurations = new List<Configuration>();
        }

        public abstract ISolution Apply(ISolution solution, Configuration configuration);

        public ISolution Random(ISolution solution)
        {
            return Apply(solution, Configurations[(random.Next(Configurations.Count))]);
        }
    }
}