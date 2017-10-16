package localsearchoptimization.examples.structures.permutation;

import localsearchoptimization.components.Configuration;
import localsearchoptimization.components.Operator;
import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.structures.TwoOperands;

public class Shift extends Operator {

    public Shift(int elementsNumber) {
        this(elementsNumber, 1);
    }

    public Shift(int elementsNumber, double weight) {
        super(elementsNumber, weight);
        for (int i = 0; i < elementsNumber; i++)
            for (int j = 0; j < elementsNumber; j++)
                if (Math.abs(i - j) >= 2) configurations.add(new TwoOperands(i, j, this));
    }

    public Solution apply(Solution solution, Configuration configuration) {
        Permutation permutation = (Permutation) solution;
        TwoOperands operands = (TwoOperands) configuration;

        int[] shifted = new int[permutation.order().length];

        shifted[operands.second] = permutation.order()[operands.first];

        for (int i = 0; i < Math.min(operands.first, operands.second); i++)
            shifted[i] = permutation.order()[i];

        for (int i = Math.max(operands.first, operands.second) + 1; i < shifted.length; i++)
            shifted[i] = permutation.order()[i];

        if (operands.first < operands.second)
            for (int i = operands.first; i < operands.second; i++)
                shifted[i] = permutation.order()[i + 1];
        else
            for (int i = operands.first; i > operands.second; i--)
                shifted[i] = permutation.order()[i - 1];

        return permutation.fetchPermutation(shifted, "shift");
    }
}