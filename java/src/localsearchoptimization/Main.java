package localsearchoptimization;

import localsearchoptimization.components.Configuration;
import localsearchoptimization.components.Operator;
import localsearchoptimization.components.Solution;
import localsearchoptimization.examples.problems.travellingsalesman.TspProblem;
import localsearchoptimization.examples.problems.travellingsalesman.TspSolution;
import localsearchoptimization.examples.structures.TwoOperands;
import localsearchoptimization.examples.structures.permutation.Shift;
import localsearchoptimization.examples.structures.permutation.Swap;
import localsearchoptimization.examples.structures.permutation.TwoOpt;
import localsearchoptimization.parameters.LocalDescentParameters;
import localsearchoptimization.solvers.LocalDescent;


public class Main {

    public static void main(String[] args) throws InterruptedException {
        TspProblem problem = new TspProblem(10);
        TspSolution solution = new TspSolution(problem);
        Swap swap = new Swap(problem.dimension, 1);
        Shift shift = new Shift(problem.dimension, 2);
        TwoOpt twoOpt = new TwoOpt(problem.dimension, 3);
        Operator[] operations = new Operator[]{twoOpt};

        LocalDescentParameters ldParameters = new LocalDescentParameters();
        ldParameters.operators = operations;
        LocalDescent ld = new LocalDescent(ldParameters);
        Solution opt = ld.minimize(solution);
        System.out.println(((TspSolution) opt).gapToLowerBound());

//        for (Configuration c : twoOpt.configurations) {
//            TwoOperands t = (TwoOperands) c;
//            if (t.first > t.second) {
//                System.out.println("    " + solution);
//                System.out.println(t.first + " " + t.second + " " + t.Apply(solution));
//            }
//        }
    }
}