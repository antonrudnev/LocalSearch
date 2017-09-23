using LocalSearch.Components;

namespace LocalSearch.Solvers
{
    public class LocalDescentParameters : SearchParameters
    {
        public bool IsSteepestDescent { get; set; } = false;

        public override SearchParameters Clone()
        {
            return new LocalDescentParameters()
            {
                Name = Name,
                Seed = Seed,
                DetailedOutput = DetailedOutput,
                Operations = Operations,
                Multistart = Multistart,
                IsSteepestDescent = IsSteepestDescent
            };
        }
    }
}