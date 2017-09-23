using LocalSearch.Solvers;
using SimpleProblems.TSP;
using System;
using System.Collections.Generic;
using System.Linq;
using LocalSearch.Components;
using SimpleProblems.Permutations;

namespace LocalSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            TspProblem problem = new TspProblem(100);
            TspSolution solution = new TspSolution(problem);

            SwapOperation swap = new SwapOperation(problem.NumberOfCities, 1);
            ShiftOperation shift = new ShiftOperation(problem.NumberOfCities, 2);
            TwoOptOperation twoOpt = new TwoOptOperation(problem.NumberOfCities, 3);

            List<Operation> operations = new List<Operation> { swap, shift, twoOpt };

            ParametersLocalDescent ldParameters = new ParametersLocalDescent()
            {
                DetailedOutput = false,
                Seed = 0,
                Operations = operations,
                Multistart = new MultistartOptions()
                {
                    InstancesNumber = 10,
                    OutputDelayInMilliseconds = 0
                }
            };

            ParametersSimulatedAnnealing saParameters = new ParametersSimulatedAnnealing()
            {
                InitProbability = 0.1,
                TemperatureCooling = 0.97,
                MaxPassesSinceLastTransition = 0.01,
                WeightNeighborhood = true,
                DetailedOutput = false,
                Seed = 0,
                Operations = operations,
                Multistart = new MultistartOptions()
                {
                    InstancesNumber = 10
                }
            };

            LocalDescent<IPermutation> ld = new LocalDescent<IPermutation>(ldParameters);
            SimulatedAnnealing<IPermutation> sa = new SimulatedAnnealing<IPermutation>(saParameters);
            ParallelSearch<IPermutation, LocalDescent<IPermutation>, ParametersLocalDescent> pld = new ParallelSearch<IPermutation, LocalDescent<IPermutation>, ParametersLocalDescent>(ldParameters);
            ParallelSearch<IPermutation, SimulatedAnnealing<IPermutation>, ParametersSimulatedAnnealing> psa = new ParallelSearch<IPermutation, SimulatedAnnealing<IPermutation>, ParametersSimulatedAnnealing>(saParameters);

            List<string> operators = new List<string>();

            IPermutation sol = solution;
            foreach (IPermutation s in pld.Minimize(solution))
            {
                Console.WriteLine("{0}, {1:f}s, {2}, {3}, {4}, {5}", s.CostValue, s.TimeInSeconds, s.IterationNumber, s.IsCurrentBest, s.IsFinal, sol.CostValue - s.CostValue);
                sol = s;
                operators.Add(s.OperationName);
            }


            var groups = operators.GroupBy(s => s).Select(s => new { Operator = s.Key, Count = s.Count() });
            var dictionary = groups.ToDictionary(g => g.Operator, g => g.Count);

            foreach (var o in groups)
            {
                Console.WriteLine("{0} = {1}", o.Operator, o.Count);
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}