using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures.Tree
{
    public class FullNodeMove : Operator
    {
        public FullNodeMove(int nodesNumber, double weight = 1) : base(weight)
        {
            for (int i = 0; i < nodesNumber; i++)
                for (int j = 0; j < 2 * nodesNumber - 1; j++)
                    Configurations.Add(new TwoOperands(i, j, this));
        }

        public override ISolution Apply(ISolution solution, Configuration configuration)
        {
            IOrientedTree tree = (IOrientedTree)solution;
            TwoOperands operands = (TwoOperands)configuration;

            List<bool> branching = new List<bool>(tree.Branching);
            List<int> order = new List<int>(tree.Order);

            int nodeOpenedAt = -1;
            int nodeClosedAt = -1;
            int opened = 0;
            int closed = 0;
            for (int i = 0; i < branching.Count; i++)
            {
                if (branching[i])
                    closed++;
                else
                    opened++;
                if (nodeOpenedAt < 0)
                {
                    if (opened == operands.First + 1)
                    {
                        nodeOpenedAt = i;
                        opened = 1;
                        closed = 0;
                    }
                }
                else if (nodeClosedAt < 0)
                    if (opened == closed)
                    {
                        nodeClosedAt = i;
                        break;
                    }
            }

            branching.RemoveAt(nodeClosedAt);
            branching.RemoveAt(nodeOpenedAt);

            int nodeOrder = 0;
            for (int i = 0; i < operands.Second; i++)
                if (!branching[i]) nodeOrder++;

            branching.Insert(operands.Second, true);
            branching.Insert(operands.Second, false);
            order.RemoveAt(operands.First);
            order.Insert(nodeOrder, tree.Order[operands.First]);
            return tree.FetchOrientedTree(order, branching, "full node");
        }
    }
}