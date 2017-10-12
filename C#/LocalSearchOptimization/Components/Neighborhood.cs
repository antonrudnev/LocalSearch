using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalSearchOptimization.Components
{
    public class Neighborhood
    {
        protected Random random;

        protected List<Operator> operators = new List<Operator>();

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
                foreach (Configuration configuration in configurations)
                {
                    yield return configuration.Apply(CurrentSolution);
                }
            }
        }

        public Neighborhood(ISolution solution, List<Operator> operators, int seed)
        {
            random = new Random(seed);
            CurrentSolution = solution;
            this.operators.AddRange(operators);
            foreach (Operator operation in operators)
                configurations.AddRange(operation.Configurations);
            Randomize();
        }

        public virtual ISolution GetRandom()
        {
            return configurations[random.Next(Power)].Apply(CurrentSolution);
        }

        public void MoveToSolution(ISolution solution)
        {
            CurrentSolution = solution;
        }

        public void Randomize()
        {
            configurations = configurations.OrderBy(x => random.Next()).ToList();
        }
    }
}