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
                if (j - i >= 3 || (i > j && elementsNumber - i + j >= 3))
                    configurations.add(new TwoOperands(i, j, this));
    }

    public Solution apply(Solution solution, Configuration configuration) {
        Permutation permutation = (Permutation) solution;
        TwoOperands operands = (TwoOperands) configuration;
        List<Integer> twoOpted;
        if (operands.first < operands.second) {
            twoOpted = new ArrayList<Integer>(permutation.order());
            reverse(twoOpted, operands.first, operands.second - operands.first);
        } else {
            twoOpted = permutation.order().subList(operands.first, permutation.order().size());
            twoOpted.addAll(permutation.order().subList(0, operands.first));
            reverse(twoOpted, 0, operands.second + twoOpted.size() - operands.first);
        }
        return permutation.fetchPermutation(twoOpted, "2opt");
    }

    private static void reverse(List<Integer> list, int index, int count) {
        for (int i = 0; i <= count / 2; i++) {
            Integer item = list.get(index + i);
            list.set(index + i, list.get(index + count - i));
            list.set(index + count - i, item);
        }
    }
}