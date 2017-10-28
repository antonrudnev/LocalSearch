package localsearchoptimization.examples.structures.permutation;

import localsearchoptimization.components.Solution;

public interface Permutation extends Solution {

    int[] order();

    Permutation fetchPermutation(int[] order, String operationName);

    default String printPermutation() {
        StringBuilder builder = new StringBuilder();
        for (int i : order())
            builder.append(i).append("->");
        return builder.toString();
    }
}