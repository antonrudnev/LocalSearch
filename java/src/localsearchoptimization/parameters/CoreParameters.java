package localsearchoptimization.parameters;

import localsearchoptimization.components.Operator;

public class CoreParameters {

    public String name = "core";

    public int seed = 0;

    public boolean isDetailedOutput = true;

    public Operator[] operators;

    public CoreParameters() {

    }

    protected CoreParameters(CoreParameters copy) {
        name = copy.name;
        seed = copy.seed;
        isDetailedOutput = copy.isDetailedOutput;
        operators = copy.operators;
    }

    public CoreParameters Clone() {
        return new CoreParameters(this);
    }
}