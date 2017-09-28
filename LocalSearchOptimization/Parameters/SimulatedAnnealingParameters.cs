namespace LocalSearchOptimization.Parameters
{
    public class SimulatedAnnealingParameters : CoreParameters
    {
        public double InitProbability { get; set; } = 0.5;

        public double TemperatureCooling { get; set; } = 0.95;

        public double TemperatureLevelPower { get; set; } = 1;

        public double MinCostDeviation { get; set; } = 10E-10;

        public int MaxFrozenLevels { get; set; } = 3;

        public bool UseWeightedNeighborhood { get; set; } = false;

        public SimulatedAnnealingParameters()
        {

        }

        protected SimulatedAnnealingParameters(SimulatedAnnealingParameters copy) : base(copy)
        {
            InitProbability = copy.InitProbability;
            TemperatureCooling = copy.TemperatureCooling;
            TemperatureLevelPower = copy.TemperatureLevelPower;
            MinCostDeviation = copy.MinCostDeviation;
            MaxFrozenLevels = copy.MaxFrozenLevels;
            UseWeightedNeighborhood = copy.UseWeightedNeighborhood;
        }

        public override CoreParameters Clone()
        {
            return new SimulatedAnnealingParameters(this);
        }
    }
}