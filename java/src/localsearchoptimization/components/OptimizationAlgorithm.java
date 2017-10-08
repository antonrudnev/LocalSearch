package localsearchoptimization.components;

import java.util.List;

public interface OptimizationAlgorithm {
    Solution currentSolution();

    List<SolutionSummary> searchHistory();

    Iterable<Solution> Minimize(Solution solution);

    void Stop();
}