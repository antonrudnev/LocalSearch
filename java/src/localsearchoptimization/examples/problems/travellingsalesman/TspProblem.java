package localsearchoptimization.examples.problems.travellingsalesman;

import java.util.Random;

public class TspProblem {
    public double[] x;
    public double[] y;
    public double[][] distance;
    public int dimension;

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
    }
}