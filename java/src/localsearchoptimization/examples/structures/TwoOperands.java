package localsearchoptimization.examples.structures;

import localsearchoptimization.components.Configuration;
import localsearchoptimization.components.Operator;

public class TwoOperands extends Configuration {

    public int first;

    public int second;

    public TwoOperands(int i, int j, Operator operation) {
        super(operation);
        first = i;
        second = j;
    }
}