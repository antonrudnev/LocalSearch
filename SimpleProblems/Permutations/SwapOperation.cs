using LocalSearch.Components;
using System.Collections.Generic;

namespace SimpleProblems.Permutations
{
    public class SwapOperation : Operation
    {
        public SwapOperation(int dimension, double weight = 1) : base(dimension, weight)
        {
            for (int i = 0; i < dimension - 1; i++)
                for (int j = i + 1; j < dimension; j++)
                    Configurations.Add(new PairConfiguration(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            IPermutation permutation = (IPermutation)solution;
            PairConfiguration pairConfiguration = (PairConfiguration)configuration;

            List<int> swapped = new List<int>(permutation.Order)
            {
                [pairConfiguration.FirstItem] = permutation.Order[pairConfiguration.SecondItem],
                [pairConfiguration.SecondItem] = permutation.Order[pairConfiguration.FirstItem]
            };

            return permutation.DeriveFromPermutation(swapped, "swap");
        }
    }
}