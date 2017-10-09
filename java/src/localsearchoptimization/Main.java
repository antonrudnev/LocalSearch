package localsearchoptimization;

import localsearchoptimization.examples.problems.travellingsalesman.TspProblem;
import localsearchoptimization.examples.problems.travellingsalesman.TspSolution;
import localsearchoptimization.examples.structures.TwoOperands;
import localsearchoptimization.examples.structures.permutation.Shift;
import localsearchoptimization.examples.structures.permutation.Swap;
import localsearchoptimization.examples.structures.permutation.TwoOpt;

public class Main {

    public static void main(String[] args) {
        int dimension = 10;
        TspSolution solution = new TspSolution(new TspProblem(dimension));
        Swap swap = new Swap(dimension);
        Shift shift = new Shift(dimension);
        TwoOpt twoOpt = new TwoOpt(dimension);
        System.out.println(twoOpt.apply(solution, new TwoOperands(8,2, twoOpt)));
    }
}