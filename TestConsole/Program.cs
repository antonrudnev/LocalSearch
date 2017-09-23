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

            SwapOperation swap = new SwapOperation(problem.NumberOfCities);
            ShiftOperation shift = new ShiftOperation(problem.NumberOfCities);
            TwoOptOperation twoOpt = new TwoOptOperation(problem.NumberOfCities);

            List<Operation> operations = new List<Operation> { swap, shift, twoOpt };

            LocalDescentParameters ldParameters = new LocalDescentParameters()
            {
                DetailedOutput = true,
                Seed = 0,
                Operations = operations,
                Multistart = new MultistartParameters()
                {
                    InstancesNumber = 10,
                    OutputDelayInMilliseconds = 0
                }
            };

            SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters()
            {
                InitProbability = 0.1,
                TemperatureCooling = 0.97,
                MaxPassesSinceLastTransition = 0.01,
                DetailedOutput = false,
                Seed = 0,
                Operations = operations,
                Multistart = new MultistartParameters()
                {
                    InstancesNumber = 10
                }
            };

            LocalDescent<IPermutation> ld = new LocalDescent<IPermutation>(ldParameters);
            SimulatedAnnealing<IPermutation> sa = new SimulatedAnnealing<IPermutation>(saParameters);
            ParallelSearch<IPermutation, LocalDescent<IPermutation>, LocalDescentParameters> pld = new ParallelSearch<IPermutation, LocalDescent<IPermutation>, LocalDescentParameters>(ldParameters);
            ParallelSearch<IPermutation, SimulatedAnnealing<IPermutation>, SimulatedAnnealingParameters> psa = new ParallelSearch<IPermutation, SimulatedAnnealing<IPermutation>, SimulatedAnnealingParameters>(saParameters);



            List<string> operators = new List<string>();

            foreach (ISolution s in pld.Minimize(solution))
            {
                Console.WriteLine("{0}, {1}, {2}", s.CostValue, s.IterationNumber, s.IsCurrentBest);
                operators.Add(((IPermutation)s).OperationName);
            }

            var groups = operators.GroupBy(s => s).Select(s => new { Operator = s.Key, Count = s.Count() });
            var dictionary = groups.ToDictionary(g => g.Operator, g => g.Count);

            foreach (var o in groups)
            {
                Console.WriteLine("{0} = {1}", o.Operator, o.Count);
            }

            //IPermutation sol = solution;
            //foreach (IPermutation s in pld.Minimize(solution))
            //{
            //    Console.WriteLine("{0}, {1:f}s, {2}, {3}, {4}, {5}", s.CostValue, s.TimeInSeconds, s.IterationNumber, s.IsCurrentBest, s.IsFinal, sol.CostValue - s.CostValue);
            //    sol = s;
            //}

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}