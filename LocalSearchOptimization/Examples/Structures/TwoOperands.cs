using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Structures

{
    public class TwoOperands : Configuration
    {
        public int Operand1 { get; }

        public int Operand2 { get; }

        public TwoOperands(int i, int j, Operator operation) : base(operation)
        {
            Operand1 = i;
            Operand2 = j;
        }
    }
}