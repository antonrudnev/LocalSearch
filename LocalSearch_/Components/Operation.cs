using System.Collections.Generic;

namespace LocalSearch.Components
{
    public abstract class Operation
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

        public Operation(int dimension, double weight = 1)
        {
            Weight = weight;
            Configurations = new List<Configuration>();
        }

        public abstract ISolution Apply(ISolution solution, Configuration configuration);

        public ISolution Apply(ISolution solution, int configuration)
        {
            return this.Apply(solution, Configurations[configuration]);
        }
    }
}