package localsearchoptimization.examples.structures.tree;

import localsearchoptimization.examples.structures.permutation.Permutation;

import java.util.List;

public interface OrientedTree extends TreeBranching, Permutation {

    OrientedTree fetchOrientedTree(List<Integer> order, List<Boolean> branching, String operationName);
}