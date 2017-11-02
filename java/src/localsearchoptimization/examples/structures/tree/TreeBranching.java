package localsearchoptimization.examples.structures.tree;

import localsearchoptimization.components.Solution;

public interface TreeBranching extends Solution {

    boolean[] getBranching();

    TreeBranching fetchBranching(boolean[] branching, String operationName);

    default String printBranching() {
        StringBuilder builder = new StringBuilder();
        for (boolean i : getBranching())
            builder.append(i ? ")" : "(");
        return builder.toString();
    }
}