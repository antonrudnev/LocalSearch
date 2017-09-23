namespace LocalSearch.Components
{
    public interface ISolution
    {
        double CostValue { get; }

        int IterationNumber { get; set; }

        double TimeInSeconds { get; set; }

        bool IsCurrentBest { get; set; }

        bool IsFinal { get; set; }

        ISolution Shuffle(int seed);
    }
}