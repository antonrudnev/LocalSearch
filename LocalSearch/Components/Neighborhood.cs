using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalSearch.Components
{
    public class Neighborhood
    {
        private Random random = new Random();

        private List<Operation> operations = new List<Operation>();

        private List<Configuration> configurations = new List<Configuration>();

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

        public Neighborhood(ISolution solution, List<Operation> operations)
        {
            CurrentSolution = solution;
            this.operations.AddRange(operations);
            double sumOperationsWeight = operations.Sum(x => x.Weight);
            this.operations.ForEach(x =>
            {
                x.Weight = x.Weight / sumOperationsWeight;
            });
            foreach (Operation operation in operations)
                this.configurations.AddRange(operation.Configurations);
            Randomize();
        }

        public ISolution GetRandom()
        {
            return configurations[random.Next(Power)].Apply(CurrentSolution);
        }

        public ISolution GetWeightedRandom()
        {
            double rand = random.NextDouble();
            double bound = 0;
            foreach (Operation operation in this.operations)
            {
                if (rand < operation.Weight + bound)
                    return operation.Random(CurrentSolution);
                bound += operation.Weight;
            }
            return operations.Last().Random(CurrentSolution);
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