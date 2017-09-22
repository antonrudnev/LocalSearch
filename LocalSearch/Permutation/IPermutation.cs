using System.Collections.Generic;

namespace LocalSearch.Components
{
    public interface IPermutation : ISolution
    {
        List<int> Permutation { get; }

        string OperationName { get; }

        IPermutation DeriveFromPermutation(List<int> permutation, string operationName);
    }
}