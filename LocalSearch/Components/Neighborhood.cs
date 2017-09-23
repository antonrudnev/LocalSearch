using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalSearch.Components
{
    public class Neighborhood
    {
        protected Random random;

        protected List<Operation> operations = new List<Operation>();

        protected List<Configuration> configurations = new List<Configuration>();

        public ISolution CurrentSolution { get; private set; }

        public int Power
        {
            get
            {
                return configurations.Count;
            }
        }

        public IEnumerable<ISolution> Neighbors
        {
            get
            {
                foreach (Configuration configuration in this.configurations)
                {
                    yield return configuration.Apply(CurrentSolution);
                }
            }
        }

        public Neighborhood(ISolution solution, List<Operation> operations, int seed)
        {
            random = new Random(seed);
            CurrentSolution = solution;
            this.operations.AddRange(operations);
            foreach (Operation operation in operations)
                this.configurations.AddRange(operation.Configurations);
            Randomize();
        }

        public virtual ISolution GetRandom()
        {
            return configurations[this.random.Next(Power)].Apply(CurrentSolution);
        }

        public void MoveToSolution(ISolution solution)
        {
            CurrentSolution = solution;
        }

        public void Randomize()
        {
            configurations = configurations.OrderBy(x => this.random.Next()).ToList();
        }
    }
}