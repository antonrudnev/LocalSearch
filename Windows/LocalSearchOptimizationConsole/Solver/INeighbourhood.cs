namespace TestConsole.Solver
{
    public interface INeighbourhood
    {
        bool IsScanned { get; }

        ISolution CurrentSolution { get; }

        ISolution GetNext();

        ISolution GetRandom();

        void InitRandom();

        double GetStartTemparature(double p);

        void MoveToSolution(ISolution solution);
    }
}