using LocalSearchOptimization.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples.Problems.TravellingSalesman;
using LocalSearchOptimization.Parameters;
using LocalSearchOptimization.Examples.Structures.Permutation;
using LocalSearchOptimization.Examples.RectangularPacking;
using LocalSearchOptimization.Examples.Structures.Tree;
using LocalSearchOptimization.Examples.Structures;

namespace LocalSearch
{
    class Program
    {
        static void Main(string[] args)
        {

            FloorplanProblem problem = new FloorplanProblem(50);
            FloorplanSolution solution = new FloorplanSolution(problem);
            Swap swap = new Swap(problem.Dimension);
            Shift shift = new Shift(problem.Dimension);
            FullLeafMove leaf = new FullLeafMove(problem.Dimension);
            List<Operator> operations = new List<Operator> { swap, shift, leaf };




            //TspProblem problem = new TspProblem(100);
            //TspSolution solution = new TspSolution(problem);
            //Swap swap = new Swap(problem.NumberOfCities, 1);
            //Shift shift = new Shift(problem.NumberOfCities, 2);
            //TwoOpt twoOpt = new TwoOpt(problem.NumberOfCities, 3);
            //List<Operator> operations = new List<Operator> { swap, shift, twoOpt };








            MultistartOptions multistartOptions = new MultistartOptions()
            {
                InstancesNumber = 10,
                OutputFrequency = 100,
                ReturnImprovedOnly = true
            };

            LocalDescentParameters ldParameters = new LocalDescentParameters()
            {
                DetailedOutput = true,
                Seed = 0,
                Operators = operations,
                IsSteepestDescent = false
            };

            SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters()
            {
                InitProbability = 0.1,
                TemperatureCooling = 0.97,
                UseWeightedNeighborhood = true,
                DetailedOutput = false,
                Seed = 0,
                Operators = operations,
            };

            LocalDescent ld = new LocalDescent(ldParameters);
            SimulatedAnnealing sa = new SimulatedAnnealing(saParameters);
            ParallelMultistart<LocalDescent, LocalDescentParameters> pld = new ParallelMultistart<LocalDescent, LocalDescentParameters>(ldParameters, multistartOptions);
            ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters> psa = new ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters>(saParameters, multistartOptions);

            List<string> operators = new List<string>();

            IPermutation sol = solution;
            foreach (IPermutation s in ld.Minimize(solution))
            {
                Console.WriteLine("{0}, {1:f}s, {2}, {3}, {4}, {5}", s.CostValue, s.TimeInSeconds, s.IterationNumber, s.IsCurrentBest, s.IsFinal, sol.CostValue - s.CostValue);
                sol = s;
                operators.Add(s.OperatorTag);
            }


            //var groups = operators.GroupBy(s => s).Select(s => new { Operator = s.Key, Count = s.Count() });
            //var dictionary = groups.ToDictionary(g => g.Operator, g => g.Count);

            //foreach (var o in groups)
            //{
            //    Console.WriteLine("{0} = {1}", o.Operator, o.Count);
            //}

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}