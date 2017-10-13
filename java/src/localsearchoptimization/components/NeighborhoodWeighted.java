package localsearchoptimization.components;

import java.util.Arrays;

public class NeighborhoodWeighted extends Neighborhood {
    public NeighborhoodWeighted(Solution solution, Operator[] operators, int seed) {
        super(solution, operators, seed);
        double sumOperationsWeight = Arrays.stream(this.operators).mapToDouble(x -> x.weight).sum();
        for (Operator operator : operators) {
            operator.weight /= sumOperationsWeight;
        }
    }

    public Solution getRandom() {
        double rand = random.nextDouble();
        double bound = 0;
        for (Operator operation : operators) {
            if (rand < operation.weight + bound)
                return operation.apply(currentSolution, random.nextInt(operation.power()));
            bound += operation.weight;
        }
        Operator lastOperation = operators[operators.length - 1];
        return lastOperation.apply(currentSolution, random.nextInt(lastOperation.power()));
    }
}