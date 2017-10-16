package localsearchoptimization.solvers;

import localsearchoptimization.components.*;
import localsearchoptimization.parameters.SimulatedAnnealingParameters;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Random;

public class SimulatedAnnealing implements OptimizationAlgorithm {

    private SimulatedAnnealingParameters parameters;

    private SolutionHandler solutionHandler;

    private Solution currentSolution;

    private ArrayList<SolutionSummary> searchHistory;

    private boolean stopFlag = false;

    public SimulatedAnnealing(SimulatedAnnealingParameters parameters) {
        this(parameters, null);
    }

    public SimulatedAnnealing(SimulatedAnnealingParameters parameters, SolutionHandler solutionHandler) {
        this.parameters = parameters;
        this.solutionHandler = solutionHandler;
    }

    @Override
    public Solution minimize(Solution startSolution) {
        stopFlag = false;
        int iteration = 0;
        Random random = new Random(parameters.seed);
        long startedAt = System.currentTimeMillis();
        Solution bestSolution = startSolution;
        Solution current = startSolution;
        currentSolution = startSolution;
        startSolution.iterationNumber(0);
        startSolution.elapsedTime(0);
        startSolution.isCurrentBest(false);
        startSolution.isFinal(false);
        startSolution.instanceTag(parameters.name);
        searchHistory = new ArrayList<SolutionSummary>(
                Arrays.asList(new SolutionSummary(parameters.name,
                        currentSolution.operatorTag(),
                        currentSolution.iterationNumber(),
                        currentSolution.cost())));
        Neighborhood neighborhood = parameters.useWeightedNeighborhood ?
                new NeighborhoodWeighted(startSolution, parameters.operators, parameters.seed) :
                new Neighborhood(startSolution, parameters.operators, parameters.seed);
        double temperature = GetStartTemperature(parameters.initProbability, neighborhood);
        int maxIterationsByTemperature = (int) (parameters.temperatureLevelPower * neighborhood.power());
        int iterationsByTemperature = 0;
        int acceptedIterationsByTemperature = 0;
        int frozenState = 0;
        double costDeviation = 0;
        while (!stopFlag && frozenState < parameters.maxFrozenLevels && temperature > 0) {
            iteration++;
            iterationsByTemperature++;
            Solution randomNeighbour = neighborhood.getRandom();
            double costDifference = randomNeighbour.cost() - current.cost();
            if (costDifference < 0 || (costDifference > 0 && random.nextDouble() < Math.exp(-costDifference / temperature))) {
                acceptedIterationsByTemperature++;
                current = randomNeighbour;
                current.iterationNumber(iteration);
                current.elapsedTime((System.currentTimeMillis() - startedAt) / 1000.0);
                current.isCurrentBest(false);
                current.isFinal(false);
                current.instanceTag(parameters.name);
                searchHistory.add(new SolutionSummary(
                        parameters.name,
                        current.operatorTag(),
                        current.iterationNumber(),
                        current.cost()
                ));
                if (current.cost() < bestSolution.cost()) {
                    current.isCurrentBest(true);
                    bestSolution = current;
                    currentSolution = bestSolution;
                    if (solutionHandler != null)
                        solutionHandler.process(currentSolution);
                } else if (parameters.isDetailedOutput) {
                    currentSolution = current;
                    if (solutionHandler != null)
                        solutionHandler.process(currentSolution);
                }
                neighborhood.moveToSolution(current);
            }
            if (iterationsByTemperature >= maxIterationsByTemperature) {
                temperature *= parameters.temperatureCooling;
                costDeviation = standardDeviation(searchHistory.subList(searchHistory.size() - acceptedIterationsByTemperature, searchHistory.size()).stream().mapToDouble(x -> x.cost).toArray());
                if (costDeviation <= parameters.minCostDeviation)
                    frozenState++;
                else
                    frozenState = 0;
                System.out.printf("\tSA %1$s cost %2$s, temp %3$s, accepted %4$d, deviation %5$s, time %6$.2fs\n", parameters.name, current.cost(), temperature, acceptedIterationsByTemperature, costDeviation, current.elapsedTime());
                iterationsByTemperature = 0;
                acceptedIterationsByTemperature = 0;
                current = current.transcode();
                neighborhood.moveToSolution(current);
            }
        }
        bestSolution.iterationNumber(iteration);
        bestSolution.elapsedTime((System.currentTimeMillis() - startedAt) / 1000.0);
        bestSolution.isFinal(true);
        currentSolution = bestSolution;
        System.out.printf("\t%1$s finished with cost %2$s, temperature %3$s, and deviation %4$s at iteration %5$d, time %6$.2fs\n", parameters.name, currentSolution.cost(), temperature, costDeviation, currentSolution.iterationNumber(), currentSolution.elapsedTime());
        if (solutionHandler != null)
            solutionHandler.process(currentSolution);
        return currentSolution;
    }

    @Override
    public Solution currentSolution() {
        return currentSolution;
    }

    @Override
    public ArrayList<SolutionSummary> searchHistory() {
        return searchHistory;
    }

    @Override
    public void stop() {
        stopFlag = true;
    }

    private double GetStartTemperature(double initProbability, Neighborhood neighborhood) {
        List<Double> temperature = new ArrayList<Double>();
        neighborhood.reset();
        while (neighborhood.hasNext()) {
            Solution solution = neighborhood.next();
            if (solution.cost() > neighborhood.currentSolution().cost())
                temperature.add(-(solution.cost() - neighborhood.currentSolution().cost()) / Math.log(initProbability));
        }
        return percentile(temperature, Math.sqrt(initProbability));
    }

    private double percentile(List<Double> sequence, double percentile) {
        double[] sorted = sequence.stream().mapToDouble(x -> x).sorted().toArray();
        int N = sorted.length;
        double n = (N - 1) * percentile + 1;
        // Another method: double n = (N + 1) * percentile;
        if (n == 1d) return sorted[0];
        else if (n == N) return sorted[N - 1];
        else {
            int k = (int) n;
            double d = n - k;
            return sorted[k - 1] + d * (sorted[k] - sorted[k - 1]);
        }
    }

    private double standardDeviation(double[] values) {
        if (values.length == 0) return 0;
        double average = 0;
        for (double x : values)
            average += x;
        average /= values.length;
        double sqrDiff = 0;
        for (double x : values)
            sqrDiff += Math.pow(x - average, 2);
        return Math.sqrt(sqrDiff / values.length);
    }
}