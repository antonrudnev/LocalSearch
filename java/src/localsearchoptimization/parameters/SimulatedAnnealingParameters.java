package localsearchoptimization.parameters;

public class SimulatedAnnealingParameters extends CoreParameters {
    public double initProbability = 0.3;

    public double temperatureCooling = 0.97;

    public double temperatureLevelPower = 1;

    public double minCostDeviation = 10E-10;

    public int maxFrozenLevels = 3;

    public boolean useWeightedNeighborhood = false;

    public SimulatedAnnealingParameters() {

    }

    protected SimulatedAnnealingParameters(SimulatedAnnealingParameters copy) {
        super(copy);
        initProbability = copy.initProbability;
        temperatureCooling = copy.temperatureCooling;
        temperatureLevelPower = copy.temperatureLevelPower;
        minCostDeviation = copy.minCostDeviation;
        maxFrozenLevels = copy.maxFrozenLevels;
        useWeightedNeighborhood = copy.useWeightedNeighborhood;
    }

    public CoreParameters Clone() {
        return new SimulatedAnnealingParameters(this);
    }
}