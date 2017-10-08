package localsearchoptimization.components;

import java.util.List;

public class NeighborhoodWeighted extends Neighborhood {
    public NeighborhoodWeighted(Solution solution, List<Operator> operators, int seed) {
        super(solution, operators, seed);
        double sumOperationsWeight = this.operators.stream().mapToDouble(x -> x.weight).sum();
        this.operators.forEach(x -> x.weight = x.weight / sumOperationsWeight);
    }

    public Solution nextRandom() {
        double rand = random.nextDouble();
        double bound = 0;
        for (Operator operation : operators) {
            if (rand < operation.weight + bound)
                return operation.Apply(currentSolution, random.nextInt(operation.power()));
            bound += operation.weight;
        }
        Operator lastOperation = operators.get(operators.size() - 1);
        return lastOperation.Apply(currentSolution, random.nextInt(lastOperation.power()));
    }
}