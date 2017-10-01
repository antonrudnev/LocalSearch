using System.Collections.Generic;
using LocalSearchOptimization.Examples.Structures.Permutation;

namespace LocalSearchOptimization.Examples.Structures.Tree
{
    public interface IOrientedTree : ITreeBranching, IPermutation
    {
        IOrientedTree FetchOrientedTree(List<int> order, List<bool> branching, string operationName);
    }
}