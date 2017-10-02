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
using System.IO;

namespace LocalSearchOptimizationConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            BitmapStyle style = new BitmapStyle()
            {
                ImageWidth = 3000,
                ImageHeight = 3000
            };

            FloorplanProblem problem = new FloorplanProblem(30);
            List<int> order = new List<int>() { 1, 2, 10, 13, 4, 30, 23, 7, 27, 14, 9, 26, 15, 29, 12, 8, 5, 28, 21, 25, 11, 17, 20, 19, 16, 3, 22, 18, 24, 6 };
            List<bool> branching = new List<bool>() { false, false, false, false, false, false, false, false, true, false, false, false, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false, false, true, false, false, true, false, false, false, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true };

            FloorplanSolution solution = new FloorplanSolution(problem, order, branching, "", 0);


            solution.Draw(style).Save("trans0.jpg");

            solution = (solution.Transcode() as FloorplanSolution);
            solution.Draw(style).Save("trans1.jpg");

            solution = (solution.Transcode() as FloorplanSolution);
            solution.Draw(style).Save("trans2.jpg");

            solution = (solution.Transcode() as FloorplanSolution);
            solution.Draw(style).Save("trans3.jpg");

            solution = (solution.Transcode() as FloorplanSolution);
            solution.Draw(style).Save("trans4.jpg");




            //Swap swap = new Swap(problem.Dimension, 10);
            //Shift shift = new Shift(problem.Dimension, 1);
            //EmptyLeafMove eLeaf = new EmptyLeafMove(problem.Dimension, 5);
            //FullLeafMove fLeaf = new FullLeafMove(problem.Dimension, 5);
            //FullNodeMove node = new FullNodeMove(problem.Dimension, 5);

            //List<Operator> operations = new List<Operator> { swap, fLeaf };



            //SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters()
            //{
            //    Name = "VLSI SA",
            //    InitProbability = 0.3,
            //    TemperatureLevelPower = 1,
            //    MinCostDeviation = 0,
            //    Seed = 0,
            //    DetailedOutput = false,
            //    Operators = operations,
            //};
            //IOptimizationAlgorithm floorplanOptimizer = new SimulatedAnnealing(saParameters);


            //foreach (ISolution s in floorplanOptimizer.Minimize(solution))
            //{
            //    Console.WriteLine(s.CostValue);
            //    if (s.OperatorTag == "bug")
            //    {
            //        (s as FloorplanSolution).Draw(style).Save("bug.jpg");
            //    }
            //}








            Console.ReadLine();
            return;


            //TspProblem problem = new TspProblem(100);
            //TspSolution solution = new TspSolution(problem);
            //Swap swap = new Swap(problem.Dimension, 1);
            //Shift shift = new Shift(problem.Dimension, 2);
            //TwoOpt twoOpt = new TwoOpt(problem.Dimension, 3);
            //List<Operator> operations = new List<Operator> { swap, shift, twoOpt };





            //MultistartOptions multistartOptions = new MultistartOptions()
            //{
            //    InstancesNumber = 20,
            //    OutputFrequency = 100,
            //    ReturnImprovedOnly = true
            //};

            //LocalDescentParameters ldParameters = new LocalDescentParameters()
            //{
            //    DetailedOutput = true,
            //    Seed = 0,
            //    Operators = operations,
            //    IsSteepestDescent = false
            //};

            //SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters()
            //{
            //    InitProbability = 0.3,
            //    TemperatureCooling = 0.97,
            //    MinCostDeviation = 10E-10,
            //    UseWeightedNeighborhood = false,
            //    DetailedOutput = true,
            //    Seed = 0,
            //    Operators = operations,
            //};

            //LocalDescent ld = new LocalDescent(ldParameters);
            //SimulatedAnnealing sa = new SimulatedAnnealing(saParameters);
            //ParallelMultistart<LocalDescent, LocalDescentParameters> pld = new ParallelMultistart<LocalDescent, LocalDescentParameters>(ldParameters, multistartOptions);
            //ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters> psa = new ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters>(saParameters, multistartOptions);
            //IOptimizationAlgorithm optimizer = sa;
            //ISolution bestSolution = solution;
            //foreach (ISolution s in optimizer.Minimize(solution))
            //{
            //    if (s.IsCurrentBest) Console.WriteLine("{0}, {1:f}s, {2}, {3}, {4}, {5}, {6}", s.CostValue, s.TimeInSeconds, s.IterationNumber, s.IsCurrentBest, s.IsFinal, bestSolution.CostValue - s.CostValue, optimizer.SearchHistory?.Count());
            //    bestSolution = s;
            //}


            //var groups = optimizer.SearchHistory.GroupBy(s => s.OperatorTag).Select(s => new { Operator = s.Key, Count = s.Count() });
            //var dictionary = groups.ToDictionary(g => g.Operator, g => g.Count);

            //foreach (var o in groups)
            //{
            //    Console.WriteLine("{0} = {1}", o.Operator, o.Count);
            //}

            //Console.WriteLine("Done");


            //foreach (var b in DrawSolution(bestSolution, optimizer))
            //{
            //    b?.Save("solution" + DateTime.Now.Millisecond + ".jpg");
            //}

            //Console.ReadLine();
        }


        static private IEnumerable<Bitmap> DrawSolution(ISolution solution, IOptimizationAlgorithm optimizer)
        {
            yield return (solution as TspSolution)?.Draw();
            yield return (solution as FloorplanSolution)?.Draw();
            yield return DrawCostDiagram.Draw(optimizer, new BitmapStyle());
        }
    }
}