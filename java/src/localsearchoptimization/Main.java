package localsearchoptimization;

import localsearchoptimization.components.*;
import localsearchoptimization.examples.problems.travellingsalesman.TspProblem;
import localsearchoptimization.examples.problems.travellingsalesman.TspSolution;
import localsearchoptimization.examples.structures.TwoOperands;
import localsearchoptimization.examples.structures.permutation.Shift;
import localsearchoptimization.examples.structures.permutation.Swap;
import localsearchoptimization.examples.structures.permutation.TwoOpt;
import localsearchoptimization.parameters.LocalDescentParameters;
import localsearchoptimization.parameters.SimulatedAnnealingParameters;
import localsearchoptimization.solvers.LocalDescent;
import localsearchoptimization.solvers.SimulatedAnnealing;

import javax.imageio.ImageIO;
import java.awt.*;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;


public class Main {

    public static void main(String[] args) throws InterruptedException, IOException {
        TspProblem problem = new TspProblem(50);
        System.out.println(problem.lowerBound);
        TspSolution solution = new TspSolution(problem);
        Swap swap = new Swap(problem.dimension, 1);
        Shift shift = new Shift(problem.dimension, 2);
        TwoOpt twoOpt = new TwoOpt(problem.dimension, 3);
        Operator[] operations = new Operator[]{swap, shift, twoOpt};

        LocalDescentParameters ldParameters = new LocalDescentParameters();
        ldParameters.operators = operations;
        LocalDescent ld = new LocalDescent(ldParameters);

        SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters();
        saParameters.initProbability = 0.5;
        saParameters.temperatureCooling = 0.94;
        saParameters.minCostDeviation = 10E-5;
        saParameters.seed = 0;
        saParameters.operators = operations;


        SimulatedAnnealing sa = new SimulatedAnnealing(saParameters);
        OptimizationAlgorithm optimizer;

        optimizer = sa;

        Solution opt = optimizer.minimize(solution);


        System.out.println(opt.elapsedTime());
        System.out.println(((TspSolution) opt).lowerBoundGap());
//
//        for (Configuration c : twoOpt.configurations) {
//            TwoOperands t = (TwoOperands) c;
//            if (t.first > t.second) {
//                System.out.println("    " + solution);
//                System.out.println(t.first + " " + t.second + " " + t.Apply(solution));
//            }
//        }

        File solfile = new File("solution.png");
        File costfile = new File("cost.png");
        ImageIO.write(((TspSolution) opt).draw(new ImageStyle()), "png", solfile);
        ImageIO.write(optimizer.drawCost(), "png", costfile);
    }
}