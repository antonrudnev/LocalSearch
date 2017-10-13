package localsearchoptimization.examples.structures.permutation;

import localsearchoptimization.components.Configuration;
import localsearchoptimization.components.Operator;
import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.structures.TwoOperands;

public class Swap extends Operator {
    public Swap(int elementsNumber) {
        this(elementsNumber, 1);
    }

    public Swap(int elementsNumber, double weight) {
        super(elementsNumber, weight);
        for (int i = 0; i < elementsNumber - 1; i++)
            for (int j = i + 1; j < elementsNumber; j++)
                configurations.add(new TwoOperands(i, j, this));
    }

    public Solution apply(Solution solution, Configuration configuration) {
        Permutation permutation = (Permutation) solution;
        TwoOperands operands = (TwoOperands) configuration;

        int[] swapped = permutation.order().clone();

        swapped[operands.first] = permutation.order()[operands.second];
        swapped[operands.second] = permutation.order()[operands.first];

        return permutation.fetchPermutation(swapped, "swap");
    }
}