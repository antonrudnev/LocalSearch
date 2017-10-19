using LocalSearchOptimization.Components;
using System.Collections.Generic;

namespace LocalSearchOptimization.Parameters
{
    public class SimulatedAnnealingParameters : OptimizationParameters
    {
        public double InitProbability { get; set; } = 0.3;

        public double TemperatureCooling { get; set; } = 0.97;

        public double TemperatureLevelPower { get; set; } = 1;

        public double MinCostDeviation { get; set; } = 10E-10;

        public int MaxFrozenLevels { get; set; } = 3;

        public bool UseWeightedNeighborhood { get; set; } = false;

        public List<Operator> Operators { get; set; }

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
            Operators = copy.Operators;
        }

        public override OptimizationParameters Clone()
        {
            return new SimulatedAnnealingParameters(this);
        }
    }
}