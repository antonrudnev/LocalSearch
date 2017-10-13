package localsearchoptimization.examples.structures.permutation;

import localsearchoptimization.components.Configuration;
import localsearchoptimization.components.Operator;
import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.structures.TwoOperands;

import java.util.ArrayList;
import java.util.List;

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
        int[] twoOpt = new int[permutation.order().length];

        if (operands.first < operands.second) {
            for (int i = 0; i < Math.min(operands.first, operands.second); i++)
                twoOpt[i] = permutation.order()[i];

            for (int i = Math.max(operands.first, operands.second) + 1; i < twoOpt.length; i++)
                twoOpt[i] = permutation.order()[i];

            for (int i = operands.first; i <= operands.second; i++)
                twoOpt[i] = permutation.order()[operands.second - (i - operands.first)];
        } else {
            for (int i = operands.second + 1; i < operands.first; i++)
                twoOpt[i - operands.second - 1] = permutation.order()[i];

            for (int i = 0; i <= operands.second; i++)
                twoOpt[operands.first - i - 1] = permutation.order()[i];

            for (int i = operands.first; i < twoOpt.length; i++)
                twoOpt[operands.first - 1 + twoOpt.length - i] = permutation.order()[i];
        }

        return permutation.fetchPermutation(twoOpt, "2opt");
    }
}