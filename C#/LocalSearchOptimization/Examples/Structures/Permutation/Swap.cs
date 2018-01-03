using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures.Permutation
{
    public class Swap : Operator
    {
        public Swap(int elementsNumber, double weight = 1) : base(weight)
        {
            for (int i = 0; i < elementsNumber - 1; i++)
                for (int j = i + 1; j < elementsNumber; j++)
                    Configurations.Add(new TwoOperands(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            IPermutation permutation = (IPermutation)solution;
            TwoOperands operands = (TwoOperands)configuration;

            List<int> swapped = new List<int>(permutation.Order)
            {
                [operands.First] = permutation.Order[operands.Second],
                [operands.Second] = permutation.Order[operands.First]
            };

            return permutation.FetchPermutation(swapped, "swap");
        }
    }
}