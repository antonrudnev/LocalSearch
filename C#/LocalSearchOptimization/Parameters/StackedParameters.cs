using System;

namespace LocalSearchOptimization.Parameters
{
    public class StackedParameters : OptimizationParameters
    {
        public Type[] OptimizationAlgorithms;

        public OptimizationParameters[] Parameters;

        public StackedParameters()
        {

        }

        protected StackedParameters(StackedParameters copy) : base(copy)
        {
            OptimizationAlgorithms = copy.OptimizationAlgorithms;
            Parameters = copy.Parameters;
        }

        public override OptimizationParameters Clone()
        {
            return new StackedParameters(this);
        }
    }
}