using System;

namespace LocalSearchOptimization.Parameters
{
    public class MultistartParameters : OptimizationParameters
    {
        public int InstancesNumber { get; set; } = 1;

        public bool RandomizeStart = true;

        public int OutputFrequency { get; set; } = 100;

        public Type OptimizationAlgorithm;

        public OptimizationParameters Parameters;

        public MultistartParameters()
        {

        }

        protected MultistartParameters(MultistartParameters copy) : base(copy)
        {
            InstancesNumber = copy.InstancesNumber;
            RandomizeStart = copy.RandomizeStart;
            OutputFrequency = copy.OutputFrequency;
            OptimizationAlgorithm = copy.OptimizationAlgorithm;
            Parameters = copy.Parameters;
        }

        public override OptimizationParameters Clone()
        {
            return new MultistartParameters(this);
        }
    }
}