package localsearchoptimization.components;

import java.util.List;

public interface OptimizationAlgorithm {
    Solution currentSolution();

    List<SolutionSummary> searchHistory();

    Solution minimize(Solution startSolution);

    void stop();
}