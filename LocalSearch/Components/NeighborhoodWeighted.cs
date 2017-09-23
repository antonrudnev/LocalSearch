using System.Collections.Generic;
using System.Linq;

namespace LocalSearch.Components
{
    public class NeighborhoodWeighted : Neighborhood
    {

        public NeighborhoodWeighted(ISolution solution, List<Operation> operations, int seed) : base(solution, operations, seed)
        {
            double sumOperationsWeight = this.operations.Sum(x => x.Weight);
            this.operations.ForEach(x =>
            {
                x.Weight = x.Weight / sumOperationsWeight;
            });
        }

        public override ISolution GetRandom()
        {
            double rand = this.random.NextDouble();
            double bound = 0;
            foreach (Operation operation in this.operations)
            {
                if (rand < operation.Weight + bound)
                    return operation.Apply(CurrentSolution, this.random.Next(operation.Power));
                bound += operation.Weight;
            }
            Operation lastOperation = operations.Last();
            return lastOperation.Apply(CurrentSolution, this.random.Next(lastOperation.Configurations.Count));
        }
    }
}