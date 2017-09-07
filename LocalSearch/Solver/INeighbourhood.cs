namespace LocalSearch.Solver
{
    public interface INeighbourhood
    {
        int Power { get; }

        double initProba { get; }

        double tempDecrease { get; }

        int tempLevelIterations { get; }

        int frozenRate { get; }

        int isLocalOptimaIterations { get; }

        ISolution CurrentSolution { get; }

        bool IsScanned { get; }

        ISolution GetNext();

        ISolution GetRandom();

        void Reset();

        void InitToRandom();

        void MoveToSolution(ISolution solution);
    }
}