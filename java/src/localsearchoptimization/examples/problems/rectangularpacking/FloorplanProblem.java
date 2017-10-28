package localsearchoptimization.examples.problems.rectangularpacking;

import localsearchoptimization.examples.problems.travellingsalesman.TspProblem;

import java.io.*;
import java.util.Random;

public class FloorplanProblem {
    public double[] w;
    public double[] h;

    public double totalArea;

    public int dimension;

    public FloorplanProblem(int numberOfRectangles) {
        this(numberOfRectangles, 0);
    }

    public FloorplanProblem(int numberOfRectangles, int seed) {
        dimension = numberOfRectangles;
        w = new double[numberOfRectangles + 1];
        h = new double[numberOfRectangles + 1];
        Random random = new Random(seed);
        for (int i = 1; i <= numberOfRectangles; i++) {
            w[i] = random.nextDouble() + 0.1;
            h[i] = random.nextDouble() + 0.1;
        }
        CalculateMetrics();
    }

    public FloorplanProblem(double[] w, double[] h) {
        if (w.length != h.length) throw new IllegalArgumentException("Arrays w and h have different lengths.");
        dimension = w.length - 1;
        this.w = w;
        this.h = h;
        CalculateMetrics();
    }

    private void CalculateMetrics() {
        w[0] = 0;
        h[0] = 0;
        totalArea = 0;
        for (int i = 1; i <= dimension; i++)
            totalArea += w[i] * h[i];
    }

    public void save(String fileName) throws FileNotFoundException, UnsupportedEncodingException {
        try (PrintWriter file = new PrintWriter(fileName, "UTF-8")) {
            file.println(dimension);
            for (int i = 1; i <= dimension; i++) {
                file.printf("%1$s %2$s\n", w[i], h[i]);
            }
        }
    }

    public static FloorplanProblem load(String fileName) throws IOException {
        try (BufferedReader file = new BufferedReader(new FileReader(fileName))) {
            int dimension = Integer.parseInt(file.readLine());
            double[] w = new double[dimension + 1];
            double[] h = new double[dimension + 1];
            for (int i = 1; i <= dimension; i++) {
                String[] wh = file.readLine().split(" ");
                w[i] = Double.parseDouble(wh[0]);
                h[i] = Double.parseDouble(wh[1]);
            }
            return new FloorplanProblem(w, h);
        }
    }
}