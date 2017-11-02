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
        startSolution.setIterationNumber(0);
        startSolution.setElapsedTime(0);
        startSolution.isCurrentBest(true);
        startSolution.isFinal(false);
        startSolution.setInstanceTag(parameters.name);
        searchHistory = new ArrayList<SolutionSummary>(
                Arrays.asList(new SolutionSummary(parameters.name,
                        currentSolution.getOperatorTag(),
                        currentSolution.getIterationNumber(),
                        currentSolution.getCost())));
        Neighborhood neighborhood = new Neighborhood(currentSolution, parameters.operators, parameters.seed);
        boolean bestFound;
        do {
            bestFound = true;
            while (neighborhood.hasNext()) {
                iteration++;
                Solution neighbor = neighborhood.getNext();
                if (neighbor.getCost() < currentSolution.getCost()) {
                    currentSolution = neighbor;
                    bestFound = false;
                    if (!parameters.isSteepestDescent) break;
                }
            }
            if (!bestFound) {
                currentSolution.setIterationNumber(iteration);
                currentSolution.setElapsedTime((System.currentTimeMillis() - startedAt) / 1000.0);
                currentSolution.isCurrentBest(true);
                currentSolution.isFinal(false);
                currentSolution.setInstanceTag(parameters.name);
                searchHistory.add(new SolutionSummary(
                        parameters.name,
                        currentSolution.getOperatorTag(),
                        currentSolution.getIterationNumber(),
                        currentSolution.getCost()
                ));
                if (parameters.isDetailedOutput && solutionHandler != null)
                    solutionHandler.process(currentSolution);
                neighborhood.moveToSolution(currentSolution);
            }
        } while (!(bestFound || stopFlag));
        currentSolution.setIterationNumber(iteration);
        currentSolution.setElapsedTime((System.currentTimeMillis() - startedAt) / 1000.0);
        currentSolution.isFinal(true);
        System.out.printf("\t%1$s finished with getCost %2$s at iteration %3$d, time %4$.2f\n", parameters.name, currentSolution.getCost(), iteration, currentSolution.getElapsedTime());
        if (solutionHandler != null)
            solutionHandler.process(currentSolution);
        return currentSolution;
    }

    @Override
    public Solution getCurrentSolution() {
        return currentSolution;
    }

    @Override
    public ArrayList<SolutionSummary> getSolutionsHistory() {
        return searchHistory;
    }

    @Override
    public void stop() {
        stopFlag = true;
    }
}