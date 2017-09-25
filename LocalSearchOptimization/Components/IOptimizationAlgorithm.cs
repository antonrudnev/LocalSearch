using System.Collections.Generic;

namespace LocalSearchOptimization.Components
{
    public interface IOptimizationAlgorithm
    {
        IEnumerable<ISolution> Minimize(ISolution solution);
    }
}