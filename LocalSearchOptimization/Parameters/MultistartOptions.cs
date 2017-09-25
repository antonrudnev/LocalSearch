namespace LocalSearchOptimization.Parameters
{
    public class MultistartOptions
    {
        public int InstancesNumber { get; set; } = 1;

        public int OutputFrequency { get; set; } = 100;

        public bool ReturnImprovedOnly { get; set; } = true;
    }
}