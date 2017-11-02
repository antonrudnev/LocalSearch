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

    public Solution getCurrentSolution() {
        return currentSolution;
    }

    public int getPower() {
        return configurations.size();
    }

    public Solution getNext() {
        return configurations.get(currentConfiguration++).apply(currentSolution);
    }

    public boolean hasNext() {
        return currentConfiguration < configurations.size();
    }

    public Solution getRandom() {
        return configurations.get(random.nextInt(getPower())).apply(currentSolution);
    }

    public void moveToSolution(Solution solution) {
        currentConfiguration = 0;
        currentSolution = solution;
    }

    public void randomize() {
        currentConfiguration = 0;
        Collections.shuffle(configurations, random);
    }

    public void reset() {
        currentConfiguration = 0;
    }
}