using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures.Permutation
{
    public interface IPermutation : ISolution
    {
        List<int> Order { get; }

        IPermutation FetchPermutation(List<int> permutation, string operationName);
    }
}