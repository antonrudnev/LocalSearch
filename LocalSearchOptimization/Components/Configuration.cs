namespace LocalSearchOptimization.Components
{
    public abstract class Configuration
    {
        private Operator operation;

        public Configuration(Operator operation)
        {
            this.operation = operation;
        }

        public ISolution Apply(ISolution solution)
        {
            return this.operation.Apply(solution, this);
        }
    }
}