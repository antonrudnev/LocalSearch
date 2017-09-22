namespace LocalSearch.Solver
{
    public class SimulatedAnnealingParameters
    {
        public int Multistart { get; set; } = 1;

        public double InitProbability { get; set; } = 0.5;

        public double TemperatureCooling { get; set; } = 0.95;

        public double TemperatureLevelPasses { get; set; } = 1;

        public double MaxPassesSinceLastTransition { get; set; } = 0.05;

        public bool CompleteWithLocalDescent { get; set; } = false;

        public bool OutputImprovementsOnly { get; set; } = true;

        public int OutputDelayInMilliseconds { get; set; } = 100;
    }
}
