package localsearchoptimization.examples.structures.tree;

import localsearchoptimization.components.Solution;

import java.util.List;

public interface TreeBranching extends Solution {
    List<Boolean> Branching();

    TreeBranching fetchBranching(List<Boolean> branching, String operationName);
}