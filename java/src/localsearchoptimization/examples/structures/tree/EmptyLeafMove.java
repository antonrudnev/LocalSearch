package localsearchoptimization.examples.structures.tree;

import localsearchoptimization.components.Configuration;
import localsearchoptimization.components.Operator;
import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.structures.TwoOperands;

public class EmptyLeafMove extends Operator {

    public EmptyLeafMove(int nodesNumber) {
        this(nodesNumber, 1);
    }

    public EmptyLeafMove(int nodesNumber, double weight) {
        super(nodesNumber, weight);
        for (int i = 0; i < 2 * nodesNumber; i++)
            for (int j = 0; j < 2 * nodesNumber - 1; j++)
                configurations.add(new TwoOperands(i, j, this));
    }

    @Override
    public Solution apply(Solution solution, Configuration configuration) {
        TreeBranching tree = (TreeBranching) solution;
        TwoOperands operands = (TwoOperands) configuration;

        int start = operands.first;
        int traverse = tree.branching()[start] == false ? 1 : -1;

        while (tree.branching()[start] == tree.branching()[start + traverse])
            start += traverse;
        int left = traverse > 0 ? start : start - 1;

        boolean[] shifted;
        if (left != operands.second) {
            shifted = new boolean[tree.branching().length];
            shifted[operands.second] = false;
            shifted[operands.second + 1] = true;
            if (left < operands.second) {
                for (int i = 0; i < left; i++)
                    shifted[i] = tree.branching()[i];
                for (int i = left + 2; i < operands.second + 2; i++)
                    shifted[i - 2] = tree.branching()[i];
                for (int i = operands.second + 2; i < shifted.length; i++)
                    shifted[i] = tree.branching()[i];
            } else {
                for (int i = 0; i < operands.second; i++)
                    shifted[i] = tree.branching()[i];
                for (int i = operands.second; i < left; i++)
                    shifted[i + 2] = tree.branching()[i];
                for (int i = left + 2; i < shifted.length; i++)
                    shifted[i] = tree.branching()[i];
            }
        } else {
            shifted = tree.branching();
        }

        return tree.fetchBranching(shifted, "empty leaf");
    }
}