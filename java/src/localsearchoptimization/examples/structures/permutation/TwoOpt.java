package localsearchoptimization.examples.structures.permutation;

import localsearchoptimization.components.Configuration;
import localsearchoptimization.components.Operator;
import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.structures.TwoOperands;

public class TwoOpt extends Operator {

    public TwoOpt(int elementsNumber) {
        this(elementsNumber, 1);
    }

    public TwoOpt(int elementsNumber, double weight) {
        super(elementsNumber, weight);
        for (int i = 0; i < elementsNumber; i++)
            for (int j = 0; j < elementsNumber; j++)
                if (Math.abs(j - i) >= 3)
                    configurations.add(new TwoOperands(i, j, this));
    }

    public Solution apply(Solution solution, Configuration configuration) {
        Permutation permutation = (Permutation) solution;
        TwoOperands operands = (TwoOperands) configuration;

        int[] twoOpted = new int[permutation.order().length];

        if (operands.first < operands.second) {
            for (int i = 0; i < Math.min(operands.first, operands.second); i++)
                twoOpted[i] = permutation.order()[i];

            for (int i = Math.max(operands.first, operands.second) + 1; i < twoOpted.length; i++)
                twoOpted[i] = permutation.order()[i];

            for (int i = operands.first; i <= operands.second; i++)
                twoOpted[i] = permutation.order()[operands.second - (i - operands.first)];
        } else {
            for (int i = 0; i <= operands.second; i++)
                twoOpted[operands.second - i] = permutation.order()[i];

            for (int i = operands.first; i < twoOpted.length; i++)
                twoOpted[operands.second + twoOpted.length - i] = permutation.order()[i];

            for (int i = operands.second + 1; i < operands.first; i++)
                twoOpted[twoOpted.length - operands.first + i] = permutation.order()[i];
        }

        return permutation.fetchPermutation(twoOpted, "2opt");
    }
}