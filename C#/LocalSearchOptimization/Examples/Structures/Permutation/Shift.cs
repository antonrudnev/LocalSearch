﻿using System;
using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures.Permutation
{
    public class Shift : Operator
    {
        public Shift(int elementsNumber, double weight = 1) : base(weight)
        {
            for (int i = 0; i < elementsNumber; i++)
                for (int j = 0; j < elementsNumber; j++)
                    if (Math.Abs(i - j) >= 2) Configurations.Add(new TwoOperands(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            IPermutation permutation = (IPermutation)solution;
            TwoOperands operands = (TwoOperands)configuration;

            List<int> shifted = new List<int>(permutation.Order);

            int shiftedItem = shifted[operands.First];
            shifted.RemoveAt(operands.First);
            if (operands.Second <= operands.First)
                shifted.Insert(operands.Second, shiftedItem);
            else
                shifted.Insert(operands.Second - 1, shiftedItem);

            return permutation.FetchPermutation(shifted, "shift");
        }
    }
}