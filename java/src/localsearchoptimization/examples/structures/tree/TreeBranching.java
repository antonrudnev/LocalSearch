package localsearchoptimization.examples.structures.tree;

import localsearchoptimization.components.Solution;

public interface TreeBranching extends Solution {

    boolean[] branching();

    TreeBranching fetchBranching(boolean[] branching, String operationName);

    default String printBranching() {
        StringBuilder builder = new StringBuilder();
        for (boolean i : branching())
            builder.append(i ? ")" : "(");
        return builder.toString();
    }
}