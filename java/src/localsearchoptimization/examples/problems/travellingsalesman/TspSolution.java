package localsearchoptimization.examples.problems.travellingsalesman;

import localsearchoptimization.components.ImageStyle;
import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.structures.permutation.Permutation;

import java.awt.*;
import java.awt.image.BufferedImage;
import java.util.Arrays;
import java.util.Collections;
import java.util.Random;
import java.util.stream.IntStream;

public class TspSolution implements Permutation {

    private TspProblem tspProblem;
    private double cost;
    private int iterationNumber;
    private double timeInSeconds;
    private boolean isCurrentBest;
    private boolean isFinal;
    private int[] order;
    private String instanceTag;
    private String operatorTag;

    public double[] x;
    public double[] y;

    public double maxWidth;
    public double maxHeight;

    public TspSolution(TspProblem tspProblem) {
        this(tspProblem, IntStream.range(0, tspProblem.dimension).toArray(), "init");
    }

    private TspSolution(TspProblem tspProblem, int[] order, String operatorName) {
        this.tspProblem = tspProblem;
        x = tspProblem.x;
        y = tspProblem.y;
        this.order = order;
        operatorTag = operatorName;
        decodeSolution(tspProblem);
    }

    @Override
    public Permutation fetchPermutation(int[] order, String operatorName) {
        return new TspSolution(tspProblem, order, operatorName);
    }

    @Override
    public Solution shuffle(int seed) {
        int[] shuffled = order.clone();
        Collections.shuffle(Arrays.asList(shuffled), new Random(seed));
        return new TspSolution(this.tspProblem, shuffled, "shuffle");
    }

    @Override
    public Solution transcode() {
        return this;
    }

    public int numberOfCities() {
        return tspProblem.dimension;
    }

    @Override
    public int[] getPermutation() {
        return order;
    }

    @Override
    public double getCost() {
        return cost;
    }

    @Override
    public int getIterationNumber() {
        return iterationNumber;
    }

    @Override
    public void setIterationNumber(int iteration) {
        iterationNumber = iteration;
    }

    @Override
    public double getElapsedTime() {
        return timeInSeconds;
    }

    @Override
    public void setElapsedTime(double seconds) {
        timeInSeconds = seconds;
    }

    @Override
    public boolean isCurrentBest() {
        return isCurrentBest;
    }

    @Override
    public void isCurrentBest(boolean isCurrentBest) {
        this.isCurrentBest = isCurrentBest;
    }

    @Override
    public boolean isFinal() {
        return isFinal;
    }

    @Override
    public void isFinal(boolean isFinal) {
        this.isFinal = isFinal;
    }

    @Override
    public String getInstanceTag() {
        return instanceTag;
    }

    @Override
    public void setInstanceTag(String tag) {
        instanceTag = tag;
    }

    @Override
    public String getOperatorTag() {
        return operatorTag;
    }

    private void decodeSolution(TspProblem problem) {
        int city = order[problem.dimension - 1];
        int nextCity = order[0];
        double cost = problem.distance[city][nextCity];
        maxWidth = problem.x[city];
        maxHeight = problem.y[city];
        for (int i = 0; i < problem.dimension - 1; i++) {
            city = order[i];
            nextCity = order[i + 1];
            cost += problem.distance[city][nextCity];
            if (maxWidth < problem.x[i]) maxWidth = problem.x[i];
            if (maxHeight < problem.y[i]) maxHeight = problem.y[i];
        }
        this.cost = cost;
    }

    public double lowerBoundGap() {
        return (cost / tspProblem.lowerBound - 1) * 100;
    }

    public BufferedImage draw(ImageStyle style) {
        double maxSize = Math.max(maxWidth, maxHeight);
        double scaleX = (style.imageWidth - style.marginX - 8 * style.radius) / maxSize;
        double scaleY = (style.imageHeight - style.marginY - 4 * style.radius) / maxSize;
        int[] xPoints = new int[tspProblem.dimension];
        int[] yPoints = new int[tspProblem.dimension];
        for (int i = 0; i < tspProblem.dimension; i++) {
            xPoints[i] = (int) (style.marginX + 2 * style.radius + x[order[i]] * scaleX);
            yPoints[i] = (int) (style.imageHeight - 2 * style.radius - y[order[i]] * scaleY);
        }
        BufferedImage bitmap = new BufferedImage(style.imageWidth, style.imageHeight, BufferedImage.TYPE_INT_RGB);
        Graphics2D g = bitmap.createGraphics();
        g.setPaint(style.backgroundColor);
        g.fillRect(0, 0, bitmap.getWidth(), bitmap.getHeight());
        g.setStroke(new BasicStroke(style.penWidth));
        if (isFinal) {
            g.setPaint(new GradientPaint(0, 0, style.fillColor, bitmap.getWidth(), bitmap.getHeight(), style.backgroundColor));
            g.fillPolygon(xPoints, yPoints, tspProblem.dimension);
        }
        if (operatorTag != "init") {
            g.setPaint(style.penColor);
            g.drawPolygon(xPoints, yPoints, tspProblem.dimension);
        }
        int smallFontSize = (int) (0.9 * style.fontSize);
        g.setFont(new Font(style.fontName, Font.PLAIN, smallFontSize));
        for (int i = 0; i < tspProblem.dimension; i++) {
            g.setPaint(new GradientPaint(xPoints[i] - style.radius, yPoints[i] - style.radius, style.backgroundColor, xPoints[i] + style.radius, yPoints[i] + style.radius, style.fillColor));
            g.fillOval(xPoints[i] - style.radius, yPoints[i] - style.radius, 2 * style.radius, 2 * style.radius);
            g.setPaint(style.penColor);
            g.drawOval(xPoints[i] - style.radius, yPoints[i] - style.radius, 2 * style.radius, 2 * style.radius);
            g.drawString(String.valueOf(order[i] + 1), xPoints[i], yPoints[i] + smallFontSize);
        }
        g.setPaint(Color.BLACK);
        g.setFont(new Font(style.fontName, Font.PLAIN, style.fontSize));
        g.drawString(String.format("Tour lenght: %1$.4f (lower bound gap %2$.2f%%)%3$s", cost, lowerBoundGap(), isCurrentBest ? " <<<" : ""), 0, style.fontSize);
        g.drawString(String.format("Iterations: %1$d", iterationNumber), 0, 2 * style.fontSize);
        g.drawString(String.format("Time: %1$.3fs", timeInSeconds), 0, 3 * style.fontSize);
        return bitmap;
    }

    @Override
    public String toString() {
        return printPermutation();
    }
}