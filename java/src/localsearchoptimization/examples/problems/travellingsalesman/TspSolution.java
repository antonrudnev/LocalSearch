package localsearchoptimization.examples.problems.travellingsalesman;

import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.structures.permutation.Permutation;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Random;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

public class TspSolution implements Permutation {
    private TspProblem tspProblem;
    private double cost;
    private int iterationNumber;
    private double timeInSeconds;
    private boolean isCurrentBest;
    private boolean isFinal;
    private List<Integer> order;
    private String instanceTag;
    private String operatorTag;

    public double[] x;
    public double[] y;

    public double maxWidth;
    public double maxHeight;

    public TspSolution(TspProblem tspProblem) {
        this(tspProblem, IntStream.range(0, tspProblem.dimension).boxed().collect(Collectors.toList()), "init");
    }

    private TspSolution(TspProblem tspProblem, List<Integer> order, String operatorName) {
        this.tspProblem = tspProblem;
        x = tspProblem.x;
        y = tspProblem.y;
        this.order = order;
        operatorTag = operatorName;
        decodeSolution(tspProblem);
    }

    public Permutation fetchPermutation(List<Integer> order, String operatorName) {
        return new TspSolution(tspProblem, order, operatorName);
    }

    public Solution shuffle(int seed) {
        List<Integer> shuffled = new ArrayList<Integer>(order);
        Collections.shuffle(shuffled, new Random(seed));
        return new TspSolution(this.tspProblem, shuffled, "shuffle");
    }

    public Solution transcode() {
        return this;
    }

    public int numberOfCities() {
        return tspProblem.dimension;
    }

    public List<Integer> order() {
        return order;
    }

    public double cost() {
        return cost;
    }

    public int iterationNumber() {
        return iterationNumber;
    }

    public void iterationNumber(int iteration) {
        iterationNumber = iteration;
    }

    public double elapsedTime() {
        return timeInSeconds;
    }

    public void elapsedTime(double seconds) {
        timeInSeconds = seconds;
    }

    public boolean isCurrentBest() {
        return isCurrentBest;
    }

    public void isCurrentBest(boolean isCurrentBest) {
        this.isCurrentBest = isCurrentBest;
    }

    public boolean isFinal() {
        return isFinal;
    }

    public void isFinal(boolean isFinal) {
        this.isFinal = isFinal;
    }

    public String instanceTag() {
        return instanceTag;
    }

    public void instanceTag(String tag) {
        instanceTag = tag;
    }

    public String operatorTag() {
        return operatorTag;
    }

    private void decodeSolution(TspProblem problem) {
        int city = order.get(problem.dimension - 1);
        int nextCity = order.get(0);
        double cost = problem.distance[city][nextCity];
        maxWidth = problem.x[city];
        maxHeight = problem.y[city];
        for (int i = 0; i < problem.dimension - 1; i++) {
            city = order.get(i);
            nextCity = order.get(i + 1);
            cost += problem.distance[city][nextCity];
            if (maxWidth < problem.x[i]) maxWidth = problem.x[i];
            if (maxHeight < problem.y[i]) maxHeight = problem.y[i];
        }
        this.cost = cost;
    }

    public double gapToLowerBound() {
        int n = tspProblem.dimension;
        double lb1 = (5.0 / 8 + 19.0 / 5184) * Math.sqrt(n);
        double lb2 = 0.7080 * Math.sqrt(n) + 0.522;
        double lb3 = 0.7078 * Math.sqrt(n) + 0.551;
        double g1 = cost / lb1 - 1;
        double g2 = cost / lb2 - 1;
        double g3 = cost / lb3 - 1;
        g1 = g1 * (1 + Math.signum(g1)) / 2 + n * (1 - Math.signum(g1)) / 2;
        g2 = g2 * (1 + Math.signum(g2)) / 2 + n * (1 - Math.signum(g2)) / 2;
        g3 = g3 * (1 + Math.signum(g3)) / 2 + n * (1 - Math.signum(g3)) / 2;
        return Math.min(g1, Math.min(g1, g3)) * 100;
    }

    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        for (int i : order) {
            builder.append(i).append("->");
        }
        return builder.toString();
    }
}