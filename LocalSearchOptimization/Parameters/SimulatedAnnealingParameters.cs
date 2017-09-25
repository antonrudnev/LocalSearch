namespace LocalSearchOptimization.Parameters
{
    public class SimulatedAnnealingParameters : CoreParameters
    {
        public double InitProbability { get; set; } = 0.5;

        public double TemperatureCooling { get; set; } = 0.95;

        public double TemperatureLevelPasses { get; set; } = 1;

        public double MaxPassesSinceLastTransition { get; set; } = 0.1;

        public bool UseWeightedNeighborhood { get; set; } = false;

        public SimulatedAnnealingParameters()
        {

        }

        protected SimulatedAnnealingParameters(SimulatedAnnealingParameters copy) : base(copy)
        {
            InitProbability = copy.InitProbability;
            TemperatureCooling = copy.TemperatureCooling;
            TemperatureLevelPasses = copy.TemperatureLevelPasses;
            MaxPassesSinceLastTransition = copy.MaxPassesSinceLastTransition;
            UseWeightedNeighborhood = copy.UseWeightedNeighborhood;
        }

        public override CoreParameters Clone()
        {
            return new SimulatedAnnealingParameters(this);
        }
    }
}