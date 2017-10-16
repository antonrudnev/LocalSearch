package localsearchoptimization.examples.problems.travellingsalesman;

import java.io.*;
import java.util.Random;

public class TspProblem {

    public final double[] x;
    public final double[] y;
    public double[][] distance;
    public int dimension;
    public double lowerBound;

    public TspProblem(int numberOfCities) {
        this(numberOfCities, 3);
    }

    public TspProblem(int numberOfCities, int seed) {
        dimension = numberOfCities;
        x = new double[dimension];
        y = new double[dimension];
        Random random = new Random(seed);
        for (int i = 0; i < dimension; i++) {
            x[i] = random.nextDouble();
            y[i] = random.nextDouble();
        }
        calculateMetrics();
    }

    public TspProblem(double[] x, double[] y) {
        if (x.length != y.length) throw new IllegalArgumentException("Arrays x and y have different lengths.");
        dimension = x.length;
        this.x = x;
        this.y = y;
        calculateMetrics();
    }

    private void calculateMetrics() {
        distance = new double[dimension][dimension];
        for (int i = 0; i < dimension - 1; i++)
            for (int j = i + 1; j < dimension; j++) {
                distance[i][j] = Math.sqrt(Math.pow(x[i] - x[j], 2) + Math.pow(y[i] - y[j], 2));
                distance[j][i] = distance[i][j];
            }

        double lb1 = 0.7080 * Math.sqrt(dimension) + 0.522;
        double lb2 = 0.7078 * Math.sqrt(dimension) + 0.551;
        lowerBound = Math.max(lb1, lb2);
    }

    public void save(String fileName) throws FileNotFoundException, UnsupportedEncodingException {
        try (PrintWriter file = new PrintWriter(fileName, "UTF-8")) {
            file.println(dimension);
            for (int i = 0; i < dimension; i++) {
                file.printf("%1$s %2$s\n", x[i], y[i]);
            }
        }
    }

    public static TspProblem load(String fileName) throws IOException {
        try (BufferedReader file = new BufferedReader(new FileReader(fileName))) {
            int dimension = Integer.parseInt(file.readLine());
            double[] x = new double[dimension];
            double[] y = new double[dimension];
            for (int i = 0; i < dimension; i++) {
                String[] xy = file.readLine().split(" ");
                x[i] = Double.parseDouble(xy[0]);
                y[i] = Double.parseDouble(xy[1]);
            }
            return new TspProblem(x, y);
        }
    }
}