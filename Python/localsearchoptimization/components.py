from abc import ABC, abstractmethod


class Solution(ABC):
    @abstractmethod
    def get_cost(self):
        pass


class OptimizationAlgorithm(ABC):
    @abstractmethod
    def get_current_solution(self):
        pass

    @abstractmethod
    def get_search_history(self):
        pass

    @abstractmethod
    def minimize(self, start_solution):
        pass

    @abstractmethod
    def stop(self):
        pass


class Operator(ABC):
    def __init__(self, weight):
        self.weight = weight
        self.configurations = []

    def get_weight(self):
        return self.weight

    def get_configurations(self):
        return self.configurations

    def get_power(self):
        return len(self.configurations)

    @abstractmethod
    def apply(self, solution, configuration):
        pass

    def apply_ind(self, solution, index):
        return self.apply(solution, self.configurations[index])


class Configuration:
    def __init__(self, operator):
        self.operator = operator

    def apply(self, solution):
        return self.operator.apply(solution, self)
