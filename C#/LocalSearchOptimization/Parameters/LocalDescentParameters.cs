namespace LocalSearchOptimization.Parameters
{
    public class LocalDescentParameters : CoreParameters
    {
        public bool IsSteepestDescent { get; set; } = false;

        public LocalDescentParameters()
        {

        }

        protected LocalDescentParameters(LocalDescentParameters copy) : base(copy)
        {
            IsSteepestDescent = copy.IsSteepestDescent;
        }

        public override CoreParameters Clone()
        {
            return new LocalDescentParameters(this);
        }
    }
}