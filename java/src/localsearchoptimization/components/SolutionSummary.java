package localsearchoptimization.components;

public class SolutionSummary {
    public String instance;

    public String operator;

    public int iteration;

    public double cost;

    public SolutionSummary(String instance, String operator, int iteration, double cost) {
        this.instance = instance;
        this.operator = operator;
        this.iteration = iteration;
        this.cost = cost;
    }
}