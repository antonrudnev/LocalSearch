package localsearchoptimization.components;

import java.util.ArrayList;
import java.util.List;

public abstract class Operator {
    public double weight;

    public List<Configuration> configurations;

    public int power() {
        return configurations.size();
    }

    public Operator(int dimension, double weight) {
        this.weight = weight;
        configurations = new ArrayList<Configuration>();
    }

    public abstract Solution Apply(Solution solution, Configuration configuration);

    public Solution Apply(Solution solution, int configuration) {
        return this.Apply(solution, configurations.get(configuration));
    }
}