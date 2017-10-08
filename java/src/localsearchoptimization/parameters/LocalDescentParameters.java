package localsearchoptimization.parameters;

public class LocalDescentParameters extends CoreParameters {
    public boolean isSteepestDescent = false;

    public LocalDescentParameters() {

    }

    protected LocalDescentParameters(LocalDescentParameters copy) {
        super(copy);
        isSteepestDescent = copy.isSteepestDescent;
    }

    public CoreParameters Clone() {
        return new LocalDescentParameters(this);
    }
}