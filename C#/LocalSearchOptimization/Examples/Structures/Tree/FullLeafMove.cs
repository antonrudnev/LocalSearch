using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures.Tree
{
    public class FullLeafMove : Operator
    {
        public FullLeafMove(int nodesNumber, double weight = 1) : base(weight)
        {
            for (int i = 0; i < 2 * nodesNumber; i++)
                for (int j = 0; j < 2 * nodesNumber - 1; j++)
                    Configurations.Add(new TwoOperands(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            IOrientedTree tree = (IOrientedTree)solution;
            TwoOperands operands = (TwoOperands)configuration;

            List<bool> branching = tree.Branching;
            List<int> order = tree.Order;

            int start = operands.First;
            int traverse = branching[start] == false ? 1 : -1;

            while (branching[start] == branching[start + traverse])
                start += traverse;

            int left = traverse > 0 ? start : start - 1;

            if (left != operands.Second)
            {
                branching = new List<bool>(tree.Branching);
                order = new List<int>(tree.Order);

                int leafOrder = 0;
                for (int i = 0; i < left; i++)
                    if (!branching[i]) leafOrder++;

                branching.RemoveRange(left, 2);

                int newOrder = 0;
                for (int i = 0; i < operands.Second; i++)
                    if (!branching[i]) newOrder++;

                branching.Insert(operands.Second, true);
                branching.Insert(operands.Second, false);

                order.RemoveAt(leafOrder);
                order.Insert(newOrder, tree.Order[leafOrder]);
            }

            return tree.FetchOrientedTree(order, branching, "full leaf");
        }
    }
}