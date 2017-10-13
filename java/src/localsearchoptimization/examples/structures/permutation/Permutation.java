package localsearchoptimization.examples.structures.permutation;

import localsearchoptimization.components.Solution;

public interface Permutation extends Solution {
    int[] order();

    Permutation fetchPermutation(int[] order, String operationName);
}