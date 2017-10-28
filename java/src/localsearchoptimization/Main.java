package localsearchoptimization;

import localsearchoptimization.components.*;
import localsearchoptimization.examples.problems.rectangularpacking.FloorplanProblem;
import localsearchoptimization.examples.problems.rectangularpacking.FloorplanSolution;
import localsearchoptimization.examples.problems.travellingsalesman.TspProblem;
import localsearchoptimization.examples.problems.travellingsalesman.TspSolution;
import localsearchoptimization.examples.structures.TwoOperands;
import localsearchoptimization.examples.structures.permutation.Shift;
import localsearchoptimization.examples.structures.permutation.Swap;
import localsearchoptimization.examples.structures.permutation.TwoOpt;
import localsearchoptimization.examples.structures.tree.EmptyLeafMove;
import localsearchoptimization.parameters.LocalDescentParameters;
import localsearchoptimization.parameters.MultistartParameters;
import localsearchoptimization.parameters.SimulatedAnnealingParameters;
import localsearchoptimization.solvers.LocalDescent;
import localsearchoptimization.solvers.ParallelMultistart;
import localsearchoptimization.solvers.SimulatedAnnealing;

import javax.imageio.ImageIO;
import java.io.File;
import java.io.IOException;
import java.util.LinkedList;
import java.util.ListIterator;

class SolutionProcessor implements SolutionHandler {
    @Override
    public void process(Solution solution) {
        System.out.printf("Cost %1$s\n", solution.cost());
    }
}

public class Main {

    public static void main(String[] args) throws InterruptedException, IOException {

//        TspProblem problem = TspProblem.load("../img/200.tsp");
//        TspSolution solution = new TspSolution(problem);

        FloorplanProblem problem = new FloorplanProblem(100);
        FloorplanSolution solution = new FloorplanSolution(problem);

        Swap swap = new Swap(problem.dimension, 1);
        Shift shift = new Shift(problem.dimension, 2);
        TwoOpt twoOpt = new TwoOpt(problem.dimension, 3);
        EmptyLeafMove emptyLeaf = new EmptyLeafMove(problem.dimension);

        Operator[] operations = new Operator[]{swap, shift, emptyLeaf};

        LocalDescentParameters ldParameters = new LocalDescentParameters();
        ldParameters.isDetailedOutput = true;
        ldParameters.operators = operations;
        LocalDescent ld = new LocalDescent(ldParameters, new SolutionProcessor());

        SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters();
        saParameters.initProbability = 0.5;
        saParameters.temperatureCooling = 0.97;
        saParameters.minCostDeviation = 0;
        saParameters.seed = 0;
        saParameters.isDetailedOutput = false;
        saParameters.operators = operations;
        SimulatedAnnealing sa = new SimulatedAnnealing(saParameters, new SolutionProcessor());

        MultistartParameters multistart = new MultistartParameters();
        multistart.instancesNumber = 1;
        multistart.outputFrequency = 500;
        multistart.isDetailedOutput = true;
        multistart.optimizationAlgorithm = LocalDescent.class;
        multistart.parameters = ldParameters;

        ParallelMultistart pm = new ParallelMultistart(multistart, new SolutionProcessor());

        OptimizationAlgorithm optimizer = sa;


//        System.out.println(solution);
//        for(int i =0; i<emptyLeaf.configurations.size();i++) {
//            TwoOperands t = (TwoOperands) emptyLeaf.configurations.get(i);
//            FloorplanSolution fs = (FloorplanSolution) t.Apply(solution);
//            System.out.printf("%1$d %2$d %3$s\n", t.first, t.second, fs);
//        }


        Solution opt = optimizer.minimize(solution);

        System.out.println("Final cost: "+opt.cost());
        System.out.println("Final time: "+opt.elapsedTime());
        System.out.println("Final iteration: "+opt.iterationNumber());


        File solfile = new File("solution.png");
        File solfile2 = new File("solution2.png");
        File solfile3 = new File("solution3.png");
        File solfile4 = new File("solution4.png");
        File costfile = new File("cost.png");
        ImageIO.write(((FloorplanSolution) opt).draw(new ImageStyle()), "png", solfile);
        ImageIO.write(((FloorplanSolution) opt.transcode()).draw(new ImageStyle()), "png", solfile2);
        ImageIO.write(((FloorplanSolution) opt.transcode().transcode()).draw(new ImageStyle()), "png", solfile3);
        ImageIO.write(((FloorplanSolution) opt.transcode().transcode().transcode()).draw(new ImageStyle()), "png", solfile4);
        ImageIO.write(optimizer.drawCost(), "png", costfile);
    }
}