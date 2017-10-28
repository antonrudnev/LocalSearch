package localsearchoptimization.examples.problems.rectangularpacking;

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
        w[0] = 0;
        h[0] = 0;
        totalArea = 0;
        for (int i = 1; i <= numberOfRectangles; i++) {
            w[i] = random.nextDouble() + 0.1;
            h[i] = random.nextDouble() + 0.1;
            totalArea += w[i] * h[i];
        }
    }
}