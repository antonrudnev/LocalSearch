using LocalSearch.Solver;
using LocalSearch.TSP;
using System;
using System.Collections.Generic;
using System.Linq;
using LocalSearch.Components;

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

            LocalDescent<IPermutation> localDescent = new LocalDescent<IPermutation>(new List<Operation> { swap, shift, twoOpt });

            SimulatedAnnealing<IPermutation> simulatedAnnealing = new SimulatedAnnealing<IPermutation>(new List<Operation> { swap, shift, twoOpt });

            //List<string> operators = new List<string>();

            //foreach (ISolution s in localDescent.Minimize(solution, new LocalDescentParameters() { Multistart = 1, OutputImprovementsOnly = true, IsSteepestDescent = false }))
            //{
            //    Console.WriteLine("{0}, {1}, {2}", s.CostValue, s.TimeInSeconds, s.IsCurrentBest);
            //    if (s.IsCurrentBest) Console.WriteLine();
            //    operators.Add(((IPermutation)s).OperationName);
            //}

            //var groups = operators.GroupBy(s => s).Select(s => new { Operator = s.Key, Count = s.Count() });
            //var dictionary = groups.ToDictionary(g => g.Operator, g => g.Count);

            //foreach (var o in groups)
            //{
            //    Console.WriteLine("{0} = {1}", o.Operator, o.Count);
            //}

            IPermutation sol = solution;
            foreach (IPermutation s in localDescent.Minimize(solution, new LocalDescentParameters() { Multistart = 10, OutputImprovementsOnly=false }))
            {
                Console.WriteLine("{0}, {1:f}s, {2}, {3}, {4}, {5}", s.CostValue, s.TimeInSeconds, s.IterationNumber, s.IsCurrentBest, s.IsFinal, sol.CostValue - s.CostValue);
                sol = s;
            }

            foreach (IPermutation s in simulatedAnnealing.Minimize(sol, new SimulatedAnnealingParameters()
            {
                Multistart = 6,
                InitProbability = 0.01,
                TemperatureCooling = 0.97,
                MaxPassesSinceLastTransition = 0.05
            }))
            {
                Console.WriteLine("{0}, {1:f}s, {2}, {3}, {4}, {5}", s.CostValue, s.TimeInSeconds, s.IterationNumber, s.IsCurrentBest, s.IsFinal, sol.CostValue - s.CostValue);
                sol = s;
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}