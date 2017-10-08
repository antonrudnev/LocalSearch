using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures

{
    public class TwoOperands : Configuration
    {
        public int First { get; }

        public int Second { get; }

        public TwoOperands(int i, int j, Operator operation) : base(operation)
        {
            First = i;
            Second = j;
        }
    }
}