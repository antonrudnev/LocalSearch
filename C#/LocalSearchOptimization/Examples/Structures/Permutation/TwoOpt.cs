using System;
using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures.Permutation
{
    public class TwoOpt : Operator
    {
        public TwoOpt(int elementsNumber, double weight = 1) : base(elementsNumber, weight)
        {
            for (int i = 0; i < elementsNumber; i++)
                for (int j = 0; j < elementsNumber; j++)
                    if (Math.Abs(i - j) >= 3) Configurations.Add(new TwoOperands(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            IPermutation permutation = (IPermutation)solution;
            TwoOperands operands = (TwoOperands)configuration;

            List<int> twoOpted;

            if (operands.First < operands.Second)
            {
                twoOpted = new List<int>(permutation.Order);
                twoOpted.Reverse(operands.First, operands.Second - operands.First + 1);
            }
            else
            {
                twoOpted = permutation.Order.GetRange(operands.First, permutation.Order.Count - operands.First);
                twoOpted.AddRange(permutation.Order.GetRange(0, operands.First));
                twoOpted.Reverse(0, operands.Second + twoOpted.Count - operands.First + 1);
            }

            return permutation.FetchPermutation(twoOpted, "2opt");
        }
    }
}