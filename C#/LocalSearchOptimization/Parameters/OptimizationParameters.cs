namespace LocalSearchOptimization.Parameters
{
    public class OptimizationParameters
    {
        public string Name { get; set; } = "core";

        public int Seed { get; set; } = 0;

        public bool DetailedOutput { get; set; } = false;

        public OptimizationParameters()
        {

        }

        protected OptimizationParameters(OptimizationParameters copy)
        {
            Name = copy.Name;
            Seed = copy.Seed;
            DetailedOutput = copy.DetailedOutput;
        }

        public virtual OptimizationParameters Clone()
        {
            return new OptimizationParameters(this);
        }
    }
}