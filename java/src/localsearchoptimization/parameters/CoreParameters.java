package localsearchoptimization.parameters;

import localsearchoptimization.components.Operator;

import java.util.List;

public class CoreParameters {
    public String name = "core";

    public int seed = 0;

    public boolean isDetailedOutput = true;

    public List<Operator> Operators;

    public CoreParameters() {

    }

    protected CoreParameters(CoreParameters copy) {
        name = copy.name;
        seed = copy.seed;
        isDetailedOutput = copy.isDetailedOutput;
        Operators = copy.Operators;
    }

    public CoreParameters Clone() {
        return new CoreParameters(this);
    }
}