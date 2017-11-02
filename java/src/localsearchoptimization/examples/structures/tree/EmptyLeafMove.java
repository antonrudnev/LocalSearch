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

        boolean[] branching = tree.getBranching();

        int start = operands.first;
        int traverse = branching[start] == false ? 1 : -1;

        while (branching[start] == branching[start + traverse])
            start += traverse;
        int left = traverse > 0 ? start : start - 1;

        if (left != operands.second) {
            branching = new boolean[tree.getBranching().length];
            branching[operands.second] = false;
            branching[operands.second + 1] = true;
            if (left < operands.second) {
                for (int i = 0; i < left; i++)
                    branching[i] = tree.getBranching()[i];
                for (int i = left + 2; i < operands.second + 2; i++)
                    branching[i - 2] = tree.getBranching()[i];
                for (int i = operands.second + 2; i < branching.length; i++)
                    branching[i] = tree.getBranching()[i];
            } else {
                for (int i = 0; i < operands.second; i++)
                    branching[i] = tree.getBranching()[i];
                for (int i = operands.second; i < left; i++)
                    branching[i + 2] = tree.getBranching()[i];
                for (int i = left + 2; i < branching.length; i++)
                    branching[i] = tree.getBranching()[i];
            }
        }

        return tree.fetchBranching(branching, "empty leaf");
    }
}