package localsearchoptimization.examples.structures.permutation;

import localsearchoptimization.components.Solution;

import java.util.List;

public interface Permutation extends Solution {
    List<Integer> order();

    Permutation fetchPermutation(List<Integer> order, String operationName);
}