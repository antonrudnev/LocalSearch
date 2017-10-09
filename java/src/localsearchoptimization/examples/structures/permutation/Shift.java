package localsearchoptimization.examples.structures.permutation;

import localsearchoptimization.components.Configuration;
import localsearchoptimization.components.Operator;
import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.structures.TwoOperands;

import java.util.ArrayList;
import java.util.List;

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
        List<Integer> shifted = new ArrayList<Integer>(permutation.order());
        Integer shiftedItem = shifted.get(operands.first);
        shifted.remove(operands.first);
        if (operands.second <= operands.first)
            shifted.add(operands.second, shiftedItem);
        else
            shifted.add(operands.second - 1, shiftedItem);
        return permutation.fetchPermutation(shifted, "shift");
    }
}