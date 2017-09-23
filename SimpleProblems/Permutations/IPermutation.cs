using LocalSearch.Components;
using System.Collections.Generic;

namespace SimpleProblems.Permutations
{
    public interface IPermutation : ISolution
    {
        List<int> Order { get; }

        string OperationName { get; }

        IPermutation DeriveFromPermutation(List<int> permutation, string operationName);
    }
}