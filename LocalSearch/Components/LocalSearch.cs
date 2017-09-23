using System.Collections.Generic;

namespace LocalSearch.Components
{
    public abstract class LocalSearch<T> where T : ISolution
    {
        public abstract IEnumerable<T> Minimize(T solution);
    }
}