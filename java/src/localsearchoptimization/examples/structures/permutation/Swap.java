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
        super(weight);
        for (int i = 0; i < elementsNumber - 1; i++)
            for (int j = i + 1; j < elementsNumber; j++)
                configurations.add(new TwoOperands(i, j, this));
    }

    @Override
    public Solution apply(Solution solution, Configuration configuration) {
        Permutation permutation = (Permutation) solution;
        TwoOperands operands = (TwoOperands) configuration;

        int[] swapped = permutation.getPermutation().clone();

        swapped[operands.first] = permutation.getPermutation()[operands.second];
        swapped[operands.second] = permutation.getPermutation()[operands.first];

        return permutation.fetchPermutation(swapped, "swap");
    }
}