package localsearchoptimization.parameters;

import localsearchoptimization.components.Operator;

public class LocalDescentParameters extends OptimizationParameters {

    public boolean isSteepestDescent = false;

    public Operator[] operators;

    public LocalDescentParameters() {

    }

    protected LocalDescentParameters(LocalDescentParameters copy) {
        super(copy);
        isSteepestDescent = copy.isSteepestDescent;
        operators = copy.operators;
    }

    @Override
    public OptimizationParameters Clone() {
        return new LocalDescentParameters(this);
    }
}