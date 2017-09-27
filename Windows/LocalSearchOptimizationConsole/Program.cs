using LocalSearchOptimization.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples.Problems.TravelingSalesman;
using LocalSearchOptimization.Parameters;
using LocalSearchOptimization.Examples.Structures.Permutation;
using LocalSearchOptimization.Examples.RectangularPacking;
using LocalSearchOptimization.Examples.Structures.Tree;
using LocalSearchOptimization.Examples.Structures;
using System.Drawing;
using LocalSearchOptimization.Examples.Problems;
using System.Drawing.Imaging;
using LocalSearchOptimization.Examples;

namespace LocalSearchOptimizationConsole
{
    class Program
    {
        static void Main(string[] args)
        {


            FloorplanProblem problem = new FloorplanProblem(50);
            FloorplanSolution solution = new FloorplanSolution(problem);
            Swap swap = new Swap(problem.Dimension, 7);
            Shift shift = new Shift(problem.Dimension, 1);
            Leaf leaf = new Leaf(problem.Dimension, 1);
            List<Operator> operations = new List<Operator> { swap, shift, leaf };




            //TspProblem problem = new TspProblem(100);
            //TspSolution solution = new TspSolution(problem);
            //Swap swap = new Swap(problem.Dimension, 1);
            //Shift shift = new Shift(problem.Dimension, 2);
            //TwoOpt twoOpt = new TwoOpt(problem.Dimension, 3);
            //List<Operator> operations = new List<Operator> { swap, shift, twoOpt };





            MultistartOptions multistartOptions = new MultistartOptions()
            {
                InstancesNumber = 20,
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
                MaxPassesSinceLastTransition = 0.1,
                UseWeightedNeighborhood = false,
                DetailedOutput = true,
                Seed = 0,
                Operators = operations,
            };

            LocalDescent ld = new LocalDescent(ldParameters);
            SimulatedAnnealing sa = new SimulatedAnnealing(saParameters);
            ParallelMultistart<LocalDescent, LocalDescentParameters> pld = new ParallelMultistart<LocalDescent, LocalDescentParameters>(ldParameters, multistartOptions);
            ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters> psa = new ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters>(saParameters, multistartOptions);

            List<string> operators = new List<string>();

            IPermutation sol = solution;
            foreach (IPermutation s in psa.Minimize(solution))
            {
                Console.WriteLine("{0}, {1:f}s, {2}, {3}, {4}, {5}, {6}", s.CostValue, s.TimeInSeconds, s.IterationNumber, s.IsCurrentBest, s.IsFinal, sol.CostValue - s.CostValue, s.SolutionsHistory?.Count());
                sol = s;
                operators.Add(s.OperatorTag);
            }


            var groups = operators.GroupBy(s => s).Select(s => new { Operator = s.Key, Count = s.Count() });
            var dictionary = groups.ToDictionary(g => g.Operator, g => g.Count);

            foreach (var o in groups)
            {
                Console.WriteLine("{0} = {1}", o.Operator, o.Count);
            }

            Console.WriteLine("Done");


            foreach (var b in DrawSolution(sol))
            {
                b?.Save("solution" + DateTime.Now.Millisecond + ".jpg");
            }

            Console.ReadLine();
        }


        static private IEnumerable<Bitmap> DrawSolution(ISolution solution)
        {
            yield return (solution as TspSolution)?.Draw();
            yield return (solution as FloorplanSolution)?.Draw();
            yield return DrawCostHistory.Draw(solution);
        }
    }
}