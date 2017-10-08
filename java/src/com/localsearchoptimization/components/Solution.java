package com.localsearchoptimization.components;

public interface Solution {

    public double cost();

    public int iterationNumber();

    public void iterationNumber(int iteration);

    public double elapsedTime();

    public void elapsedTime(double seconds);

    public boolean isCurrentBest();

    public void isCurrentBest(boolean isCurrentBest);

    public boolean isFinal();

    void isFinal(boolean isFinal);

    public String instanceTag();

    public void instanceTag(String tag);

    public String operatorTag();

    public Solution shuffle(int seed);

    public Solution transcode();
}