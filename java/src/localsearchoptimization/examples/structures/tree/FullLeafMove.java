package localsearchoptimization.examples.structures.tree;

import localsearchoptimization.components.Configuration;
import localsearchoptimization.components.Operator;
import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.structures.TwoOperands;

public class FullLeafMove extends Operator {

    public FullLeafMove(int nodesNumber) {
        this(nodesNumber, 1);
    }

    public FullLeafMove(int nodesNumber, double weight) {
        super(nodesNumber, weight);
        for (int i = 0; i < 2 * nodesNumber; i++)
            for (int j = 0; j < 2 * nodesNumber - 1; j++)
                configurations.add(new TwoOperands(i, j, this));
    }

    @Override
    public Solution apply(Solution solution, Configuration configuration) {
        OrientedTree tree = (OrientedTree) solution;
        TwoOperands operands = (TwoOperands) configuration;

        boolean[] branching = tree.branching();
        int[] order = tree.order();

        int start = operands.first;
        int traverse = branching[start] == false ? 1 : -1;

        while (branching[start] == branching[start + traverse])
            start += traverse;

        int left = traverse > 0 ? start : start - 1;

        if (left != operands.second) {
            int leafOrder = 0;
            int newLeafOrder = 0;
            for (int i = 0; i < Math.max(left, operands.second + (left < operands.second ? 2 : 0)); i++) {
                if (i < left && !branching[i]) leafOrder++;
                if (i < operands.second + (left < operands.second ? 2 : 0) && !branching[i] && i != left)
                    newLeafOrder++;
            }

            branching = new boolean[tree.branching().length];
            branching[operands.second] = false;
            branching[operands.second + 1] = true;
            if (left < operands.second) {
                for (int i = 0; i < left; i++)
                    branching[i] = tree.branching()[i];
                for (int i = left + 2; i < operands.second + 2; i++)
                    branching[i - 2] = tree.branching()[i];
                for (int i = operands.second + 2; i < branching.length; i++)
                    branching[i] = tree.branching()[i];
            } else {
                for (int i = 0; i < operands.second; i++)
                    branching[i] = tree.branching()[i];
                for (int i = operands.second; i < left; i++)
                    branching[i + 2] = tree.branching()[i];
                for (int i = left + 2; i < branching.length; i++)
                    branching[i] = tree.branching()[i];
            }

            order = new int[tree.order().length];
            order[newLeafOrder] = tree.order()[leafOrder];

            for (int i = 0; i < Math.min(leafOrder, newLeafOrder); i++)
                order[i] = tree.order()[i];
            for (int i = Math.max(leafOrder, newLeafOrder) + 1; i < order.length; i++)
                order[i] = tree.order()[i];
            if (leafOrder < newLeafOrder)
                for (int i = leafOrder; i < newLeafOrder; i++)
                    order[i] = tree.order()[i + 1];
            else
                for (int i = leafOrder; i > newLeafOrder; i--)
                    order[i] = tree.order()[i - 1];
        }

        return tree.fetchOrientedTree(order, branching, "full leaf");
    }
}