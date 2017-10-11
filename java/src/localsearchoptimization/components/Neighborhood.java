package localsearchoptimization.components;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Random;

public class Neighborhood {
    protected Random random;

    protected Operator[] operators;

    protected List<Configuration> configurations = new ArrayList<Configuration>();

    protected Solution currentSolution;

    protected int currentConfiguration;

    public Neighborhood(Solution solution, Operator[] operators, int seed) {
        random = new Random(seed);
        currentSolution = solution;
        this.operators = operators;
        for (Operator operation : this.operators)
            configurations.addAll(operation.configurations);
        randomize();
    }

    public Solution currentSolution() {
        return currentSolution;
    }

    public int power() {
        return configurations.size();
    }

    public Solution next() {
        return configurations.get(currentConfiguration++).Apply(currentSolution);
    }

    public boolean hasNext() {
        return currentConfiguration < configurations.size();
    }

    public Solution nextRandom() {
        return configurations.get(random.nextInt(power())).Apply(currentSolution);
    }

    public void moveToSolution(Solution solution) {
        currentConfiguration = 0;
        currentSolution = solution;
    }

    public void randomize() {
        currentConfiguration = 0;
        Collections.shuffle(configurations, random);
    }
}