using LocalSearch.Components;

namespace LocalSearch.Solvers
{
    public class SimulatedAnnealingParameters : SearchParameters
    {
        public double InitProbability { get; set; } = 0.5;

        public double TemperatureCooling { get; set; } = 0.95;

        public double TemperatureLevelPasses { get; set; } = 1;

        public double MaxPassesSinceLastTransition { get; set; } = 0.1;

        public bool WeightNeighborhood { get; set; } = false;

        public override SearchParameters Clone()
        {
            return new SimulatedAnnealingParameters()
            {
                Name = Name,
                Seed = Seed,
                DetailedOutput = DetailedOutput,
                Operations = Operations,
                Multistart = Multistart,
                InitProbability = InitProbability,
                TemperatureCooling = TemperatureCooling,
                TemperatureLevelPasses = TemperatureLevelPasses,
                MaxPassesSinceLastTransition = MaxPassesSinceLastTransition,
                WeightNeighborhood = WeightNeighborhood
            };
        }
    }
}