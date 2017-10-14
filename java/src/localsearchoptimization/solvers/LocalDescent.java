package localsearchoptimization.solvers;

import localsearchoptimization.components.Neighborhood;
import localsearchoptimization.components.OptimizationAlgorithm;
import localsearchoptimization.components.Solution;
import localsearchoptimization.components.SolutionSummary;
import localsearchoptimization.parameters.LocalDescentParameters;

import java.time.Duration;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class LocalDescent implements OptimizationAlgorithm {
    private LocalDescentParameters parameters;

    private Solution currentSolution;

    private ArrayList<SolutionSummary> searchHistory;

    private boolean stopFlag = false;

    public LocalDescent(LocalDescentParameters parameters) {
        this.parameters = parameters;
    }

    public Solution minimize(Solution startSolution) {
        stopFlag = false;
        int iteration = 0;
        LocalDateTime startedAt = LocalDateTime.now();
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
                currentSolution.elapsedTime(Duration.between(startedAt, LocalDateTime.now()).toMillis() / 1000.0);
                currentSolution.isCurrentBest(true);
                currentSolution.isFinal(false);
                currentSolution.instanceTag(parameters.name);
                searchHistory.add(new SolutionSummary(
                        parameters.name,
                        currentSolution.operatorTag(),
                        currentSolution.iterationNumber(),
                        currentSolution.cost()
                ));
                if (parameters.isDetailedOutput)
                    System.out.printf("\t%1$s updated to cost %2$s at iteration %3$d\n", parameters.name, currentSolution.cost(), iteration);
                neighborhood.moveToSolution(currentSolution);
            }
        } while (!(bestFound || stopFlag));
        currentSolution.iterationNumber(iteration);
        currentSolution.elapsedTime(Duration.between(startedAt, LocalDateTime.now()).toMillis() / 1000.0);
        currentSolution.isFinal(true);
        System.out.printf("\t%1$s finished with cost %2$s at iteration %3$d\n", parameters.name, currentSolution.cost(), iteration);
        return currentSolution;
    }

    public void stop() {
        stopFlag = true;
    }

    public Solution currentSolution() {
        return currentSolution;
    }

    public ArrayList<SolutionSummary> searchHistory() {
        return searchHistory;
    }
}