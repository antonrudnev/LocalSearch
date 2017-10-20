package localsearchoptimization.solvers;

import localsearchoptimization.components.OptimizationAlgorithm;
import localsearchoptimization.components.Solution;
import localsearchoptimization.components.SolutionHandler;
import localsearchoptimization.components.SolutionSummary;
import localsearchoptimization.parameters.OptimizationParameters;
import localsearchoptimization.parameters.MultistartParameters;

import java.lang.reflect.InvocationTargetException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Random;
import java.util.concurrent.RecursiveAction;
import java.util.stream.Collectors;

public class ParallelMultistart implements OptimizationAlgorithm {

    private MultistartParameters parameters;

    private SolutionHandler solutionHandler;

    private Solution currentSolution;

    private ArrayList<SolutionSummary> searchHistory;

    private boolean stopFlag = false;

    private final Object thisLock = new Object();

    public ParallelMultistart(MultistartParameters parameters) {
        this(parameters, null);
    }

    public ParallelMultistart(MultistartParameters parameters, SolutionHandler solutionHandler) {
        this.parameters = parameters;
        this.solutionHandler = solutionHandler;
    }

    @Override
    public Solution minimize(Solution startSolution) {
        stopFlag = false;
        Random random = new Random(parameters.seed);
        long startedAt = System.currentTimeMillis();
        Solution bestSolution = startSolution;
        currentSolution = startSolution;
        startSolution.iterationNumber(0);
        startSolution.elapsedTime(0);
        startSolution.isCurrentBest(false);
        startSolution.isFinal(false);
        startSolution.instanceTag(parameters.name);
        searchHistory = new ArrayList<SolutionSummary>();
        ArrayList<Solution> solutions = new ArrayList<Solution>();
        RecursiveAction[] solvers = new RecursiveAction[parameters.instancesNumber];
        for (int i = 0; i < parameters.instancesNumber; i++) {
            OptimizationParameters instanceParameters = parameters.parameters.Clone();
            instanceParameters.name = parameters.parameters.name + (parameters.instancesNumber > 1 ? ":" + i : "");
            instanceParameters.seed = random.nextInt();
            Solution instanceStartSolution = startSolution.shuffle(instanceParameters.seed);
            solvers[i] = new Solver(parameters.optimizationAlgorithm, instanceParameters, startSolution, solutions);
            solvers[i].fork();
        }

        while (Arrays.stream(solvers).anyMatch(x -> !x.isDone()) || solutions.size() > 0) {
            try {
                Thread.sleep(parameters.outputFrequency);
                synchronized (thisLock) {
                    if (solutions.size() > 0) {
                        Solution current = solutions.stream().sorted((f1, f2) -> Double.compare(f1.cost(), f2.cost())).findFirst().get();
                        current.elapsedTime((System.currentTimeMillis() - startedAt) / 1000.0);
                        current.isCurrentBest(false);
                        current.isFinal(false);
                        solutions.stream().map(x -> new SolutionSummary(x.instanceTag(), x.operatorTag(), x.iterationNumber(), x.cost())).collect(Collectors.toCollection(() -> searchHistory));
                        if (current.cost() < bestSolution.cost()) {
                            current.isCurrentBest(true);
                            bestSolution = current;
                            currentSolution = current;
                            if (solutionHandler != null)
                                solutionHandler.process(currentSolution);
                        } else if (parameters.isDetailedOutput) {
                            currentSolution = current;
                            if (solutionHandler != null)
                                solutionHandler.process(currentSolution);
                        }
                        solutions.clear();
                    }
                }
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
        bestSolution.iterationNumber(Arrays.stream(solvers).mapToInt(x -> ((Solver) x).solver.currentSolution().iterationNumber()).sum());
        bestSolution.elapsedTime((System.currentTimeMillis() - startedAt) / 1000.0);
        bestSolution.isCurrentBest(true);
        bestSolution.isFinal(true);
        currentSolution = bestSolution;
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

    private class Solver extends RecursiveAction {
        public OptimizationAlgorithm solver;
        private  Class optimizerType;
        private OptimizationParameters parameters;
        private Solution startSolution;
        private ArrayList<Solution> outputSolutions;

        public Solver(Class optimizerType, OptimizationParameters parameters, Solution startSolution, ArrayList<Solution> outputSolutions) {
            this.parameters = parameters;
            this.optimizerType = optimizerType;
            this.startSolution = startSolution;
            this.outputSolutions = outputSolutions;
        }

        @Override
        protected void compute() {
            try {
                solver = (OptimizationAlgorithm)optimizerType.getConstructor(parameters.getClass(), SolutionHandler.class).newInstance(parameters, new SolutionProcessor(outputSolutions));
                solver.minimize(startSolution);
            } catch (NoSuchMethodException | InvocationTargetException | InstantiationException | IllegalAccessException e) {
                e.printStackTrace();
            }
        }
    }

    private class SolutionProcessor implements SolutionHandler {

        private ArrayList<Solution> outputSolutions;

        public SolutionProcessor(ArrayList<Solution> outputSolutions) {
            this.outputSolutions = outputSolutions;
        }

        @Override
        public void process(Solution solution) {
            synchronized (thisLock) {
                outputSolutions.add(solution);
            }
        }
    }
}