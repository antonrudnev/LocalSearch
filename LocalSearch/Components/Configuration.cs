namespace LocalSearch.Components
{
    public abstract class Configuration
    {
        private Operation operation;

        public Configuration(Operation operation)
        {
            this.operation = operation;
        }

        public ISolution Apply(ISolution solution)
        {
            return this.operation.Apply(solution, this);
        }
    }
}