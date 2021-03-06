package localsearchoptimization.components;

import java.awt.*;
import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;

public interface OptimizationAlgorithm {

    Solution getCurrentSolution();

    ArrayList<SolutionSummary> getSolutionsHistory();

    Solution minimize(Solution startSolution);

    void stop();

    default BufferedImage drawCost() {
        return drawCost(new ImageStyle(), 0);
    }

    default BufferedImage drawCost(ImageStyle style) {
        return drawCost(style, 0);
    }

    default BufferedImage drawCost(ImageStyle style, int maxPoints) {
        Color[] colors = new Color[]{Color.BLUE, Color.GREEN, Color.CYAN, Color.MAGENTA, Color.ORANGE, Color.RED, Color.YELLOW, Color.PINK};
        String[] notNeighborOperators = new String[]{"init", "shuffle", "transcode"};
        if (getSolutionsHistory() == null || getSolutionsHistory().size() == 0) return null;
        int n = maxPoints == 0 ? 1 : getSolutionsHistory().size() / maxPoints + 1;
        ArrayList<SolutionSummary> historyToDraw = maxPoints == 0 ? getSolutionsHistory() : new ArrayList<SolutionSummary>();
        double minCost = Integer.MAX_VALUE;
        double maxCost = 0;
        HashSet<String> instances = new HashSet<String>();
        HashSet<String> operators = new HashSet<String>();
        for (int i = 0; i < getSolutionsHistory().size(); i++) {
            if (minCost > getSolutionsHistory().get(i).cost) minCost = getSolutionsHistory().get(i).cost;
            if (maxCost < getSolutionsHistory().get(i).cost) maxCost = getSolutionsHistory().get(i).cost;
            if (maxPoints == 0 || i % n == 0) {
                if (maxPoints > 0) historyToDraw.add(getSolutionsHistory().get(i));
                instances.add(getSolutionsHistory().get(i).instance);
                operators.add(getSolutionsHistory().get(i).operator);
            }
        }
        HashMap<String, Color> instanceBrush = new HashMap<String, Color>();
        int counter = 0;
        for (String instance : instances) {
            instanceBrush.put(instance, colors[counter % colors.length]);
            counter++;
        }
        HashMap<String, Color> operatorBrush = new HashMap<String, Color>();
        counter = 0;
        for (String operation : operators) {

            boolean flag = true;
            for (String eliminated : notNeighborOperators)
                if (eliminated.equals(operation)) flag = false;
            if (flag) {
                operatorBrush.put(operation, colors[counter % colors.length]);
                counter++;
            } else
                operatorBrush.put(operation, colors[0]);
        }
        double scaleX = (double) (style.imageWidth - style.marginX) / (historyToDraw.size());
        double scaleY = (style.imageHeight - style.marginY - 4 * style.penWidth) / (maxCost - minCost);
        BufferedImage bitmap = new BufferedImage(style.imageWidth, style.imageHeight, BufferedImage.TYPE_INT_RGB);
        Graphics2D g = bitmap.createGraphics();
        g.setPaint(style.backgroundColor);
        g.fillRect(0, 0, bitmap.getWidth(), bitmap.getHeight());
        for (int i = 0; i < historyToDraw.size() - 1; i++) {
            int x = (int) (style.marginX + i * scaleX);
            int y = (int) (bitmap.getHeight() - 4 * style.penWidth - (historyToDraw.get(i).cost - minCost) * scaleY);
            g.setPaint(instanceBrush.size() > 1 ? instanceBrush.get(historyToDraw.get(i).instance) : operatorBrush.get(historyToDraw.get(i).operator));
            g.fillOval(x, y, 2 * style.penWidth, 2 * style.penWidth);
        }
        g.setPaint(Color.BLACK);
        g.setFont(new Font(style.fontName, Font.PLAIN, style.fontSize));
        g.drawString(String.format("Max getCost: %1$.4f", maxCost), 0, style.fontSize);
        g.drawString(String.format("Min getCost: %1$.4f", minCost), 0, 2 * style.fontSize);
        g.drawString(String.format("Accepted iterations: %1$d", getSolutionsHistory().size()), 0, 3 * style.fontSize);
        g.drawString(String.format("Time: %1$.3fs", getCurrentSolution().getElapsedTime()), 0, 4 * style.fontSize);
        return bitmap;
    }
}