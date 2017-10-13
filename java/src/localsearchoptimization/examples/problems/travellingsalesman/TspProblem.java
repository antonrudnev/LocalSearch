package localsearchoptimization.examples.problems.travellingsalesman;

import java.util.Random;

public class TspProblem {
    public final double[] x;
    public final double[] y;
    public final double[][] distance;
    public final int dimension;
    public final double lowerBound;

    public TspProblem(int numberOfCities) {
        this(numberOfCities, 3);
    }

    public TspProblem(int numberOfCities, int seed) {
        dimension = numberOfCities;
        x = new double[numberOfCities];
        y = new double[numberOfCities];
        distance = new double[numberOfCities][numberOfCities];

        Random random = new Random(seed);

        for (int i = 0; i < numberOfCities; i++) {
            x[i] = random.nextDouble();
            y[i] = random.nextDouble();
        }

        for (int i = 0; i < numberOfCities - 1; i++)
            for (int j = i + 1; j < numberOfCities; j++) {
                distance[i][j] = Math.sqrt(Math.pow(x[i] - x[j], 2) + Math.pow(y[i] - y[j], 2));
                distance[j][i] = distance[i][j];
            }

        double lb1 = 0.7080 * Math.sqrt(dimension) + 0.522;
        double lb2 = 0.7078 * Math.sqrt(dimension) + 0.551;
        lowerBound = Math.max(lb1, lb2);
    }
}