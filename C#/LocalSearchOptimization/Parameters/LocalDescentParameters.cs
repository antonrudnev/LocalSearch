using LocalSearchOptimization.Components;
using System.Collections.Generic;

namespace LocalSearchOptimization.Parameters
{
    public class LocalDescentParameters : OptimizationParameters
    {
        public bool IsSteepestDescent { get; set; } = false;

        public List<Operator> Operators { get; set; }

        public LocalDescentParameters()
        {

        }

        protected LocalDescentParameters(LocalDescentParameters copy) : base(copy)
        {
            IsSteepestDescent = copy.IsSteepestDescent;
            Operators = copy.Operators;
        }

        public override OptimizationParameters Clone()
        {
            return new LocalDescentParameters(this);
        }
    }
}