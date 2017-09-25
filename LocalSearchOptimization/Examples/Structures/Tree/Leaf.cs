using System;
using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures.Tree
{
    public class Leaf : Operator
    {
        public Leaf(int nodesNumber, double weight = 1) : base(nodesNumber, weight)
        {
            for (int i = 0; i < 2 * nodesNumber; i++)
                for (int j = 0; j < 2 * nodesNumber - 2; j++)
                    if (Math.Abs(i - j) >= 1) Configurations.Add(new TwoOperands(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            ITreeStructure tree = (ITreeStructure)solution;
            TwoOperands pair = (TwoOperands)configuration;

            List<bool> shifted = new List<bool>(tree.Branching);

            int start = pair.Operand1;
            int traverse = shifted[start] == false ? 1 : -1;

            while (!(shifted[start] ^ shifted[start + traverse]))
            {
                start += traverse;
            }

            int left = traverse > 0 ? start : start - 1;

            shifted.RemoveRange(left, 2);
            shifted.Insert(pair.Operand2, true);
            shifted.Insert(pair.Operand2, false);

            return tree.FetchBranching(shifted, "leaf");
        }
    }
}