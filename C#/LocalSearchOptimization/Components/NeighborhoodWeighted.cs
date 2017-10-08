using System.Collections.Generic;
using System.Linq;

namespace LocalSearchOptimization.Components
{
    public class WeightedNeighborhood : Neighborhood
    {
        public WeightedNeighborhood(ISolution solution, List<Operator> operators, int seed) : base(solution, operators, seed)
        {
            double sumOperationsWeight = this.operators.Sum(x => x.Weight);
            this.operators.ForEach(x =>
            {
                x.Weight = x.Weight / sumOperationsWeight;
            });
        }

        public override ISolution GetRandom()
        {
            double rand = this.random.NextDouble();
            double bound = 0;
            foreach (Operator operation in operators)
            {
                if (rand < operation.Weight + bound)
                    return operation.Apply(CurrentSolution, this.random.Next(operation.Power));
                bound += operation.Weight;
            }
            Operator lastOperation = operators.Last();
            return lastOperation.Apply(CurrentSolution, this.random.Next(lastOperation.Configurations.Count));
        }
    }
}