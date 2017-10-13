package localsearchoptimization.components;

public class SolutionSummary {
    public final String instance;
    public final String operator;
    public final int iteration;
    public final double cost;

    public SolutionSummary(String instance, String operator, int iteration, double cost) {
        this.instance = instance;
        this.operator = operator;
        this.iteration = iteration;
        this.cost = cost;
    }
}