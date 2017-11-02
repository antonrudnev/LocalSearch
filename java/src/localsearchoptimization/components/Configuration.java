package localsearchoptimization.components;

public abstract class Configuration {

    private Operator operation;

    public Configuration(Operator operation) {

        this.operation = operation;
    }

    public Solution apply(Solution solution) {
        return operation.apply(solution, this);
    }
}
