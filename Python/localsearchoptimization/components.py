class Solution:
    def get_cost(self):
        raise NotImplementedError()


class OptimizationAlgorithm:
        def get_current_solution(self):
            raise NotImplementedError()

        def get_search_history(self):
            raise NotImplementedError

        def minimize(self, start_solution):
            raise NotImplementedError()

        def stop(self):
            raise NotImplementedError()


class Operator:
    def __init__(self, weight):
        self.weight = weight
        self.configurations = []

    def get_weight(self):
        return self.weight

    def get_configurations(self):
        return self.configurations

    def get_power(self):
        return len(self.configurations)

    def apply(self, solution, configuration):
        raise NotImplementedError()

    def apply_ind(self, solution, index):
        return self.apply(solution, self.configurations[index])


class Configuration:
    def __init__(self, operator):
        self.operator = operator

    def apply(self, solution):
        return self.operator.apply(solution, self)
