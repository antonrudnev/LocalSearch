using LocalSearch.Components;

namespace SimpleProblems.Permutations
{
    public class PairConfiguration : Configuration
    {
        public int FirstItem { get; }

        public int SecondItem { get; }

        public PairConfiguration(int i, int j, Operation operation) : base(operation)
        {
            FirstItem = i;
            SecondItem = j;
        }

        public override string ToString()
        {
            return FirstItem + "-" + SecondItem;
        }
    }
}