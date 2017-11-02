package localsearchoptimization.examples.structures.permutation;

import localsearchoptimization.components.Solution;

public interface Permutation extends Solution {

    int[] getPermutation();

    Permutation fetchPermutation(int[] order, String operationName);

    default String printPermutation() {
        StringBuilder builder = new StringBuilder();
        for (int i : getPermutation())
            builder.append(i).append("->");
        return builder.toString();
    }
}