package localsearchoptimization.components;

public interface Solution {

    double cost();

    int iterationNumber();

    void iterationNumber(int iteration);

    double elapsedTime();

    void elapsedTime(double seconds);

    boolean isCurrentBest();

    void isCurrentBest(boolean isCurrentBest);

    boolean isFinal();

    void isFinal(boolean isFinal);

    String instanceTag();

    void instanceTag(String tag);

    String operatorTag();

    Solution shuffle(int seed);

    Solution transcode();
}