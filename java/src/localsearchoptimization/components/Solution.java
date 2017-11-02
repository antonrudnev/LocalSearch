package localsearchoptimization.components;

public interface Solution {

    double getCost();

    int getIterationNumber();

    void setIterationNumber(int iteration);

    double getElapsedTime();

    void setElapsedTime(double seconds);

    boolean isCurrentBest();

    void isCurrentBest(boolean isCurrentBest);

    boolean isFinal();

    void isFinal(boolean isFinal);

    String getInstanceTag();

    void setInstanceTag(String tag);

    String getOperatorTag();

    Solution shuffle(int seed);

    Solution transcode();
}