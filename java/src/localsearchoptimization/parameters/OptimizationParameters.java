package localsearchoptimization.parameters;

public class OptimizationParameters {

    public String name = "core";

    public int seed = 0;

    public boolean isDetailedOutput = true;

    public OptimizationParameters() {

    }

    protected OptimizationParameters(OptimizationParameters copy) {
        name = copy.name;
        seed = copy.seed;
        isDetailedOutput = copy.isDetailedOutput;
    }

    public OptimizationParameters Clone() {
        return new OptimizationParameters(this);
    }
}