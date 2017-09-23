using LocalSearch.Components;
using System;
using System.Collections.Generic;

namespace SimpleProblems.Permutations
{
    public class ShiftOperation : Operation
    {
        public ShiftOperation(int dimension, double weight = 1) : base(dimension, weight)
        {
            for (int i = 0; i < dimension; i++)
                for (int j = 0; j < dimension; j++)
                    if (Math.Abs(i - j) >= 2) Configurations.Add(new PairConfiguration(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            IPermutation permutation = (IPermutation)solution;
            PairConfiguration pairConfiguration = (PairConfiguration)configuration;

            List<int> shifted = new List<int>(permutation.Order);

            int shiftedItem = shifted[pairConfiguration.FirstItem];
            shifted.RemoveAt(pairConfiguration.FirstItem);
            if (pairConfiguration.SecondItem <= pairConfiguration.FirstItem)
                shifted.Insert(pairConfiguration.SecondItem, shiftedItem);
            else
                shifted.Insert(pairConfiguration.SecondItem - 1, shiftedItem);

            return permutation.DeriveFromPermutation(shifted, "shift");
        }
    }
}