package localsearchoptimization;

import localsearchoptimization.components.*;
import localsearchoptimization.examples.problems.travellingsalesman.TspProblem;
import localsearchoptimization.examples.problems.travellingsalesman.TspSolution;
import localsearchoptimization.examples.structures.permutation.Shift;
import localsearchoptimization.examples.structures.permutation.Swap;
import localsearchoptimization.examples.structures.permutation.TwoOpt;
import localsearchoptimization.parameters.LocalDescentParameters;
import localsearchoptimization.parameters.MultistartOptions;
import localsearchoptimization.parameters.SimulatedAnnealingParameters;
import localsearchoptimization.solvers.LocalDescent;
import localsearchoptimization.solvers.ParallelMultistart;
import localsearchoptimization.solvers.SimulatedAnnealing;

import javax.imageio.ImageIO;
import java.io.File;
import java.io.IOException;

class SolutionProcessor implements SolutionHandler {
    @Override
    public void process(Solution solution) {
        System.out.printf("Cost %1$s\n", solution.cost());
    }
}

public class Main {

    public static void main(String[] args) throws InterruptedException, IOException {

        TspProblem problem = TspProblem.load("../img/200.tsp");
        System.out.println(problem.lowerBound);
        TspSolution solution = new TspSolution(problem);
        Swap swap = new Swap(problem.dimension, 1);
        Shift shift = new Shift(problem.dimension, 2);
        TwoOpt twoOpt = new TwoOpt(problem.dimension, 3);
        Operator[] operations = new Operator[]{swap, shift, twoOpt};

        LocalDescentParameters ldParameters = new LocalDescentParameters();
        ldParameters.isDetailedOutput = true;
        ldParameters.operators = operations;
        LocalDescent ld = new LocalDescent(ldParameters, new SolutionProcessor());

        SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters();
        saParameters.initProbability = 0.5;
        saParameters.temperatureCooling = 0.94;
        saParameters.minCostDeviation = 10E-5;
        saParameters.seed = 0;
        saParameters.isDetailedOutput = true;
        saParameters.operators = operations;
        SimulatedAnnealing sa = new SimulatedAnnealing(saParameters, new SolutionProcessor());

        MultistartOptions multistart = new MultistartOptions();
        multistart.instancesNumber = 14;
        multistart.outputFrequency = 500;
        multistart.returnImprovedOnly = true;

        ParallelMultistart<LocalDescent, LocalDescentParameters> pld = new ParallelMultistart<LocalDescent, LocalDescentParameters>(LocalDescent.class, ldParameters, multistart, new SolutionProcessor());

        ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters> psa = new ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters>(SimulatedAnnealing.class, saParameters, multistart, new SolutionProcessor());


        OptimizationAlgorithm optimizer = psa;

        Solution opt = optimizer.minimize(solution);

        System.out.println("Lower bound: "+((TspSolution) opt).lowerBoundGap());
        System.out.println("Final cost: "+opt.cost());
        System.out.println("Final time: "+opt.elapsedTime());
        System.out.println("Final iteration: "+opt.iterationNumber());

        File solfile = new File("solution.png");
        File costfile = new File("cost.png");
        ImageIO.write(((TspSolution) opt).draw(new ImageStyle()), "png", solfile);
        ImageIO.write(optimizer.drawCost(), "png", costfile);
    }
}