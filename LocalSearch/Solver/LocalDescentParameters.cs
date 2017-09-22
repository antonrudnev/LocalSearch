namespace LocalSearch.Solver
{
    public class LocalDescentParameters
    {
        public int Multistart { get; set; } = 1;

        public bool IsSteepestDescent { get; set; } = false;

        public bool OutputImprovementsOnly { get; set; } = true;

        public int OutputDelayInMilliseconds { get; set; } = 100;
    }
}