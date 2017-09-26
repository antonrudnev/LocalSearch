using System.Collections.Generic;

namespace LocalSearchOptimization.Components
{
    public interface ISolution
    {
        double CostValue { get; }

        int IterationNumber { get; set; }

        double TimeInSeconds { get; set; }

        bool IsCurrentBest { get; set; }

        bool IsFinal { get; set; }

        string OperatorTag { get; }

        string InstanceTag { get; set; }

        List<ISolution> SolutionsHistory { get; set; }

        ISolution Shuffle(int seed);
    }
}