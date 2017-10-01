using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures.Tree
{
    public class EmptyLeafMove : Operator
    {
        public EmptyLeafMove(int nodesNumber, double weight = 1) : base(nodesNumber, weight)
        {
            for (int i = 0; i < 2 * nodesNumber; i++)
                for (int j = 0; j < 2 * nodesNumber - 1; j++)
                    if (i != j) Configurations.Add(new TwoOperands(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            ITreeBranching tree = (ITreeBranching)solution;
            TwoOperands operands = (TwoOperands)configuration;

            List<bool> shifted = new List<bool>(tree.Branching);

            int start = operands.First;
            int traverse = shifted[start] == false ? 1 : -1;

            while (!(shifted[start] ^ shifted[start + traverse]))
            {
                start += traverse;
            }

            int left = traverse > 0 ? start : start - 1;

            shifted.RemoveRange(left, 2);
            shifted.Insert(operands.Second, true);
            shifted.Insert(operands.Second, false);

            return tree.FetchBranching(shifted, "empty leaf");
        }
    }
}