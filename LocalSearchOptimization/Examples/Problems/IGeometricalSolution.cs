using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Problems
{
    public interface IGeometricalSolution : ISolution
    {
        ProblemGeometry Details { get; }
    }
}