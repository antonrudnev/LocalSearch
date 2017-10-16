package localsearchoptimization.solvers;

import localsearchoptimization.components.*;
import localsearchoptimization.parameters.LocalDescentParameters;

import java.util.ArrayList;
import java.util.Arrays;

public class LocalDescent implements OptimizationAlgorithm {

    private LocalDescentParameters parameters;

    private SolutionHandler solutionHandler;

    private Solution currentSolution;

    private ArrayList<SolutionSummary> searchHistory;

    private boolean stopFlag = false;

    public LocalDescent(LocalDescentParameters parameters) {
        this(parameters, null);
    }

    public LocalDescent(LocalDescentParameters parameters, SolutionHandler solutionHandler) {
        this.parameters = parameters;
        this.solutionHandler = solutionHandler;
    }

    @Override
    public Solution minimize(Solution startSolution) {
        stopFlag = false;
        int iteration = 0;
        long startedAt = System.currentTimeMillis();
        currentSolution = startSolution;
        startSolution.iterationNumber(0);
        startSolution.elapsedTime(0);
        startSolution.isCurrentBest(true);
        startSolution.isFinal(false);
        startSolution.instanceTag(parameters.name);
        searchHistory = new ArrayList<SolutionSummary>(
                Arrays.asList(new SolutionSummary(parameters.name,
                        currentSolution.operatorTag(),
                        currentSolution.iterationNumber(),
                        currentSolution.cost())));
        Neighborhood neighborhood = new Neighborhood(currentSolution, parameters.operators, parameters.seed);
        boolean bestFound;
        do {
            bestFound = true;
            while (neighborhood.hasNext()) {
                iteration++;
                Solution neighbor = neighborhood.next();
                if (neighbor.cost() < currentSolution.cost()) {
                    currentSolution = neighbor;
                    bestFound = false;
                    if (!parameters.isSteepestDescent) break;
                }
            }
            if (!bestFound) {
                currentSolution.iterationNumber(iteration);
                currentSolution.elapsedTime((System.currentTimeMillis() - startedAt) / 1000.0);
                currentSolution.isCurrentBest(true);
                currentSolution.isFinal(false);
                currentSolution.instanceTag(parameters.name);
                searchHistory.add(new SolutionSummary(
                        parameters.name,
                        currentSolution.operatorTag(),
                        currentSolution.iterationNumber(),
                        currentSolution.cost()
                ));
                if (parameters.isDetailedOutput && solutionHandler != null)
                    solutionHandler.process(currentSolution);
                neighborhood.moveToSolution(currentSolution);
            }
        } while (!(bestFound || stopFlag));
        currentSolution.iterationNumber(iteration);
        currentSolution.elapsedTime((System.currentTimeMillis() - startedAt) / 1000.0);
        currentSolution.isFinal(true);
        System.out.printf("\t%1$s finished with cost %2$s at iteration %3$d, time %4$.2f\n", parameters.name, currentSolution.cost(), iteration, currentSolution.elapsedTime());
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
}