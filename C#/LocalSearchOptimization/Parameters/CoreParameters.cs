using System.Collections.Generic;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Parameters
{
    public class CoreParameters
    {
        public string Name { get; set; } = "core";

        public int Seed { get; set; } = 0;

        public bool DetailedOutput { get; set; } = true;

        public List<Operator> Operators { get; set; }

        public CoreParameters()
        {

        }

        protected CoreParameters(CoreParameters copy)
        {
            Name = copy.Name;
            Seed = copy.Seed;
            DetailedOutput = copy.DetailedOutput;
            Operators = copy.Operators;
        }

        public virtual CoreParameters Clone()
        {
            return new CoreParameters(this);
        }
    }
}