using System;
using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures.Permutation
{
    public class Shift : Operator
    {
        public Shift(int elementsNumber, double weight = 1) : base(elementsNumber, weight)
        {
            for (int i = 0; i < elementsNumber; i++)
                for (int j = 0; j < elementsNumber; j++)
                    if (Math.Abs(i - j) >= 2) Configurations.Add(new TwoOperands(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            IPermutation permutation = (IPermutation)solution;
            TwoOperands pair = (TwoOperands)configuration;

            List<int> shifted = new List<int>(permutation.Order);

            int shiftedItem = shifted[pair.Operand1];
            shifted.RemoveAt(pair.Operand1);
            if (pair.Operand2 <= pair.Operand1)
                shifted.Insert(pair.Operand2, shiftedItem);
            else
                shifted.Insert(pair.Operand2 - 1, shiftedItem);

            return permutation.FetchPermutation(shifted, "shift");
        }
    }
}