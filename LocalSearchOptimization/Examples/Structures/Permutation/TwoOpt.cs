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
                    if (j - i >= 3 || (i > j && elementsNumber - i + j >= 3)) Configurations.Add(new TwoOperands(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            IPermutation permutation = (IPermutation)solution;
            TwoOperands pair = (TwoOperands)configuration;

            List<int> twoOpted;

            if (pair.Operand1 < pair.Operand2)
            {
                twoOpted = new List<int>(permutation.Order);
                twoOpted.Reverse(pair.Operand1, pair.Operand2 - pair.Operand1 + 1);
            }
            else
            {
                twoOpted = permutation.Order.GetRange(pair.Operand1, permutation.Order.Count - pair.Operand1);
                twoOpted.AddRange(permutation.Order.GetRange(0, pair.Operand1));
                twoOpted.Reverse(0, pair.Operand2 + twoOpted.Count - pair.Operand1 + 1);
            }

            return permutation.FetchPermutation(twoOpted, "2opt");
        }
    }
}