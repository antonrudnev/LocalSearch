package localsearchoptimization.examples.structures.permutation;

import localsearchoptimization.components.Configuration;
import localsearchoptimization.components.Operator;
import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.structures.TwoOperands;

import java.util.ArrayList;
import java.util.List;

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
        List<Integer> swapped = new ArrayList<Integer>(permutation.order());
        swapped.set(operands.first, permutation.order().get(operands.second));
        swapped.set(operands.second, permutation.order().get(operands.first));
        return permutation.fetchPermutation(swapped, "swap");
    }
}