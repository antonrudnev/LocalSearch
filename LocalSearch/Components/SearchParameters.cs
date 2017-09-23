using System.Collections.Generic;

namespace LocalSearch.Components
{
    public abstract class SearchParameters
    {
        public string Name { get; set; } = "core";

        public int Seed { get; set; } = 0;

        public bool DetailedOutput { get; set; } = true;

        public List<Operation> Operations { get; set; }

        public MultistartParameters Multistart { get; set; }

        public abstract SearchParameters Clone();
    }
}