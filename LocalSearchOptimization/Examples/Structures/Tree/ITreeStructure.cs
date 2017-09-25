using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures.Tree
{
    public interface ITreeStructure : ISolution
    {
        List<bool> Branching { get; }

        ITreeStructure FetchBranching(List<bool> branching, string operationName);
    }
}