package localsearchoptimization.components;

import java.util.ArrayList;
import java.util.List;

public abstract class Operator {

    public double weight;

    public List<Configuration> configurations;

    public int power() {
        return configurations.size();
    }

    public Operator(double weight) {
        this.weight = weight;
        configurations = new ArrayList<Configuration>();
    }

    public abstract Solution apply(Solution solution, Configuration configuration);

    public Solution apply(Solution solution, int configuration) {
        return this.apply(solution, configurations.get(configuration));
    }
}