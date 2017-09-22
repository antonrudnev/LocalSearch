using System;
using System.Collections.Generic;

namespace LocalSearch.Components
{
    public class TwoOptOperation : Operation
    {
        public TwoOptOperation(int dimension, double weight = 1) : base(dimension, weight)
        {
            for (int i = 0; i < dimension; i++)
                for (int j = 0; j < dimension; j++)
                    if (j - i >= 3 || (i > j && dimension - i + j >= 3)) Configurations.Add(new PairConfiguration(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            IPermutation permutation = (IPermutation)solution;
            PairConfiguration pairConfiguration = (PairConfiguration)configuration;

            List<int> twoOpted;

            if (pairConfiguration.FirstItem < pairConfiguration.SecondItem)
            {
                twoOpted = new List<int>(permutation.Permutation);
                twoOpted.Reverse(pairConfiguration.FirstItem, pairConfiguration.SecondItem - pairConfiguration.FirstItem + 1);
            }
            else
            {
                twoOpted = permutation.Permutation.GetRange(pairConfiguration.FirstItem, permutation.Permutation.Count - pairConfiguration.FirstItem);
                twoOpted.AddRange(permutation.Permutation.GetRange(0, pairConfiguration.FirstItem));
                twoOpted.Reverse(0, pairConfiguration.SecondItem + twoOpted.Count - pairConfiguration.FirstItem + 1);
            }

            return permutation.DeriveFromPermutation(twoOpted, "2opt");
        }
    }
}