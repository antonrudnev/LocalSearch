using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures.Tree
{
    public interface ITreeBranching : ISolution
    {
        List<bool> Branching { get; }

        ITreeBranching FetchBranching(List<bool> branching, string operationName);
    }
}