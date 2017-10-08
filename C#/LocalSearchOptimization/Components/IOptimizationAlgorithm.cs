using System.Collections.Generic;

namespace LocalSearchOptimization.Components
{
    public interface IOptimizationAlgorithm
    {
        ISolution CurrentSolution { get; }

        List<SolutionSummary> SearchHistory { get; }

        IEnumerable<ISolution> Minimize(ISolution solution);

        void Stop();
    }
}