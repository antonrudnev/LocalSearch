package localsearchoptimization.components;

public abstract class Configuration {
    private Operator operation;

    public Configuration(Operator operation) {

        this.operation = operation;
    }

    public Solution Apply(Solution solution) {
        return operation.apply(solution, this);
    }
}
