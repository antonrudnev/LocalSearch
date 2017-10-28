package localsearchoptimization.examples.problems.rectangularpacking;

import localsearchoptimization.components.ImageStyle;
import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.structures.permutation.Permutation;
import localsearchoptimization.examples.structures.tree.OrientedTree;
import localsearchoptimization.examples.structures.tree.TreeBranching;

import java.awt.*;
import java.awt.image.BufferedImage;
import java.util.*;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

public class FloorplanSolution implements OrientedTree {

    private FloorplanProblem floorplanProblem;
    private int transcoder;
    private double cost;
    private int iterationNumber;
    private double timeInSeconds;
    private boolean isCurrentBest;
    private boolean isFinal;
    private int[] order;
    private boolean[] branching;
    private String instanceTag;
    private String operatorTag;

    public double[] x;
    public double[] y;
    public double[] w;
    public double[] h;

    public double maxWidth;
    public double maxHeight;

    public FloorplanSolution(FloorplanProblem floorplanProblem) {
        this(floorplanProblem, null, null, "init");
    }

    public FloorplanSolution(FloorplanProblem floorplanProblem, int[] order, boolean[] branching, String operatorName) {
        this(floorplanProblem, order, branching, operatorName, 0);
    }

    public FloorplanSolution(FloorplanProblem floorplanProblem, int[] order, boolean[] branching, String operatorName, int transcoder) {
        this.floorplanProblem = floorplanProblem;
        this.transcoder = transcoder;
        x = new double[floorplanProblem.dimension + 1];
        y = new double[floorplanProblem.dimension + 1];
        w = transcoder % 2 == 0 ? floorplanProblem.w : floorplanProblem.h;
        h = transcoder % 2 == 0 ? floorplanProblem.h : floorplanProblem.w;
        if (order != null)
            this.order = order;
        else
            this.order = IntStream.rangeClosed(1, floorplanProblem.dimension).toArray();
        if (branching != null)
            this.branching = branching;
        else {
            this.branching = new boolean[2 * floorplanProblem.dimension];
            boolean[] open = new boolean[floorplanProblem.dimension];
            boolean[] close = new boolean[floorplanProblem.dimension];
            Arrays.fill(open, false);
            Arrays.fill(close, true);
            System.arraycopy(open, 0, this.branching, 0, floorplanProblem.dimension);
            System.arraycopy(close, 0, this.branching, floorplanProblem.dimension, floorplanProblem.dimension);
        }
        this.operatorTag = operatorName;
        DecodeSolution();
    }

    public int numberOfRectangles() {
        return floorplanProblem.dimension;
    }

    public double itemsArea() {
        return floorplanProblem.totalArea;
    }

    public double utilization() {
        return floorplanProblem.totalArea * 100 / (maxWidth * maxHeight);
    }

    @Override
    public boolean[] branching() {
        return branching;
    }

    @Override
    public double cost() {
        return cost;
    }

    @Override
    public TreeBranching fetchBranching(boolean[] branching, String operationName) {
        return new FloorplanSolution(floorplanProblem, order, branching, operationName, transcoder);
    }

    @Override
    public OrientedTree fetchOrientedTree(int[] order, boolean[] branching, String operationName) {
        return new FloorplanSolution(floorplanProblem, order, branching, operationName, transcoder);
    }

    @Override
    public Permutation fetchPermutation(int[] order, String operationName) {
        return new FloorplanSolution(floorplanProblem, order, branching, operationName, transcoder);
    }

    @Override
    public int iterationNumber() {
        return iterationNumber;
    }

    @Override
    public void iterationNumber(int iteration) {
        iterationNumber = iteration;
    }

    @Override
    public double elapsedTime() {
        return timeInSeconds;
    }

    @Override
    public void elapsedTime(double seconds) {
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
    public String instanceTag() {
        return instanceTag;
    }

    @Override
    public void instanceTag(String tag) {
        instanceTag = tag;
    }

    @Override
    public String operatorTag() {
        return operatorTag;
    }

    @Override
    public int[] order() {
        return order;
    }

    @Override
    public Solution shuffle(int seed) {
        Random random = new Random(seed);
        boolean[] branching = new boolean[this.branching.length];
        int opened = 0;
        int completed = 0;
        int currentBranching = 0;
        for (int i = 0; i < 2 * floorplanProblem.dimension; i++) {
            if (completed < floorplanProblem.dimension && (opened == 0 || random.nextDouble() < 0.5)) {
                branching[currentBranching++] = false;
                opened++;
                completed++;
            } else {
                branching[currentBranching++] = true;
                opened--;
            }
        }
        int[] shuffled = order.clone();
        Collections.shuffle(Arrays.asList(shuffled), new Random(seed));
        return new FloorplanSolution(floorplanProblem, shuffled, branching, "shuffle", transcoder);
    }

    @Override
    public Solution transcode() {
        Stack<Integer> parents = new Stack<Integer>();
        parents.push(0);
        List<Integer> uncoded = IntStream.rangeClosed(1, floorplanProblem.dimension).boxed().collect(Collectors.toList());
        int[] order = new int[this.order.length];
        boolean[] branching = new boolean[this.branching.length];
        int opened = 0;
        int currentOrder = 0;
        int currentBranching = 0;
        while (uncoded.size() > 0) {
            int parent = parents.peek();
            int next = 0;
            double nextX = 0;
            for (int item : uncoded) {
                if ((y[item] == y[parent] + h[parent])
                        && (x[parent] < x[item] + w[item])
                        && (x[item] < x[parent] + (parent > 0 ? w[parent] : maxWidth))
                        && (x[item] >= nextX)) {
                    next = item;
                    nextX = x[item];
                }
            }
            if (next > 0) {
                order[currentOrder++] = next;
                branching[currentBranching++] = false;
                parents.push(next);
                uncoded.remove((Integer) next);
                opened++;
            } else {
                branching[currentBranching++] = true;
                parents.pop();
                opened--;
            }
        }
        for (int i = 0; i < opened; i++)
            branching[currentBranching++] = true;
        return new FloorplanSolution(floorplanProblem, order, branching, "transcode", (transcoder + 1) % 4);
    }

    private void DecodeSolution() {
        LinearDecoder();
    }

    public void LinearDecoder() {
        maxWidth = 0;
        maxHeight = 0;
        x[0] = 0;
        y[0] = 0;
        w[0] = 0;
        h[0] = 0;
        LinkedList<Integer> contourList = new LinkedList<Integer>();
        contourList.add(0);
        ListIterator<Integer> contour = contourList.listIterator();
        int perm = 0;
        for (boolean b : branching) {
            if (!b) {
                int parent = contour.next();
                int current = order[perm];
                x[current] = x[parent] + w[parent];
                double maxY = 0;
                boolean topMostFound = false;
                double rightSide = x[current] + w[current];
                while (contour.hasNext() && !topMostFound) {
                    int top = contour.next();
                    if (maxY < y[top] + h[top])
                        maxY = y[top] + h[top];
                    if (x[top] + w[top] <= rightSide)
                        contour.remove();
                    else topMostFound = true;
                }
                y[current] = maxY;
                if (topMostFound) contour.previous();
                contour.add(current);
                contour.previous();
                perm++;
                double currentW = x[current] + w[current];
                double currentH = y[current] + h[current];
                if (maxWidth < currentW)
                    maxWidth = currentW;
                if (maxHeight < currentH)
                    maxHeight = currentH;
            } else {
                contour.previous();
            }
        }
//        cost = maxWidth * maxHeight;
        cost = Math.pow(maxWidth + maxHeight, 2);
    }

    public BufferedImage draw(ImageStyle style) {
        double maxWidth = transcoder % 2 == 0 ? this.maxWidth : this.maxHeight;
        double maxHeight = transcoder % 2 == 0 ? this.maxHeight : this.maxWidth;
        double maxSize = Math.max(maxWidth, maxHeight);
        double scaleX = (style.imageWidth - style.marginX - 8 * style.radius) / maxSize;
        double scaleY = (style.imageHeight - style.marginY - 4 * style.radius) / maxSize;
        double[] x = transcoder % 2 == 0 ? this.x : this.y;
        double[] y = transcoder % 2 == 0 ? this.y : this.x;
        double[] w = transcoder % 2 == 0 ? this.w : this.h;
        double[] h = transcoder % 2 == 0 ? this.h : this.w;
        BufferedImage bitmap = new BufferedImage(style.imageWidth, style.imageHeight, BufferedImage.TYPE_INT_RGB);
        Graphics2D g = bitmap.createGraphics();
        g.setPaint(style.backgroundColor);
        g.fillRect(0, 0, bitmap.getWidth(), bitmap.getHeight());
        g.setStroke(new BasicStroke(style.penWidth));

        g.setPaint(Color.LIGHT_GRAY);
        g.fillRect((int) (style.marginX + style.penWidth), (int) (bitmap.getHeight() - style.penWidth - maxHeight * scaleY), (int) (maxWidth * scaleX), (int) (maxHeight * scaleY));

        int smallFontSize = (int) (0.9 * style.fontSize);
        g.setFont(new Font(style.fontName, Font.PLAIN, smallFontSize));

        for (int n : order) {
            int xScaled = (int) (style.marginX + style.penWidth + ((transcoder == 1 || transcoder == 2 ? maxWidth - x[n] - w[n] : x[n]) * scaleX));
            int yScaled = (int) (bitmap.getHeight() - style.penWidth - ((transcoder == 2 || transcoder == 3 ? maxHeight - y[n] : y[n] + h[n]) * scaleY));
            int wScaled = (int) (w[n] * scaleX);
            int hScaled = (int) (h[n] * scaleY);
            g.setPaint(new GradientPaint(xScaled, yScaled, style.backgroundColor, xScaled + wScaled, yScaled + hScaled, style.fillColor));
            g.fillRect(xScaled, yScaled, wScaled, hScaled);
            g.setPaint(style.penColor);
            g.drawRect(xScaled, yScaled, wScaled, hScaled);
            g.drawString(String.valueOf(n), xScaled + (wScaled - smallFontSize) / 2, yScaled + (hScaled + smallFontSize) / 2);
        }

        g.setPaint(Color.BLACK);
        g.setFont(new Font(style.fontName, Font.PLAIN, style.fontSize));
        g.drawString(String.format("Packing %1$s: %2$.4f (utilization %3$.2f%%)%4$s", cost == maxWidth * maxHeight ? "area" : "cost", cost, utilization(), isCurrentBest ? " <<<" : ""), 0, style.fontSize);
        g.drawString(String.format("Iterations: %1$d", iterationNumber), 0, 2 * style.fontSize);
        g.drawString(String.format("Time: %1$.3fs", timeInSeconds), 0, 3 * style.fontSize);
        return bitmap;
    }

    @Override
    public String toString() {
        return printTree();
    }
}