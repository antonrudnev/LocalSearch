package localsearchoptimization.parameters;

public class MultistartParameters extends OptimizationParameters {

    public int instancesNumber = 1;

    public int outputFrequency = 100;

    public Class optimizationAlgorithm;

    public OptimizationParameters parameters;

    public MultistartParameters() {

    }

    protected MultistartParameters(MultistartParameters copy) {
        super(copy);
        instancesNumber = copy.instancesNumber;
        outputFrequency = copy.outputFrequency;
        optimizationAlgorithm = copy.optimizationAlgorithm;
        parameters = copy.parameters;
    }

    @Override
    public MultistartParameters Clone() {
        return new MultistartParameters(this);
    }
}