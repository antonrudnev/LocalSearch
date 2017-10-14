﻿using LocalSearchOptimization.Solvers;
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
            //FloorplanProblem problem = new FloorplanProblem(500);
            //FloorplanSolution solution = new FloorplanSolution(problem);
            //Swap swap = new Swap(problem.Dimension, 10);
            //Shift shift = new Shift(problem.Dimension, 1);
            //EmptyLeafMove eLeaf = new EmptyLeafMove(problem.Dimension, 5);
            //FullLeafMove fLeaf = new FullLeafMove(problem.Dimension, 5);
            //FullNodeMove node = new FullNodeMove(problem.Dimension, 5);
            //List<Operator> operations = new List<Operator> { swap, fLeaf };



            TspProblem problem = new TspProblem(200);
            TspSolution solution = new TspSolution(problem);
            Swap swap = new Swap(problem.Dimension, 1);
            Shift shift = new Shift(problem.Dimension, 2);
            TwoOpt twoOpt = new TwoOpt(problem.Dimension, 3);
            List<Operator> operations = new List<Operator> { swap, shift, twoOpt };


            MultistartOptions multistartOptions = new MultistartOptions()
            {
                InstancesNumber = 50,
                OutputFrequency = 1000,
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
                InitProbability = 0.5,
                TemperatureCooling = 0.94,
                MinCostDeviation = 10E-5,
                UseWeightedNeighborhood = false,
                DetailedOutput = false,
                Seed = 0,
                Operators = operations,
            };

            LocalDescent ld = new LocalDescent(ldParameters);
            SimulatedAnnealing sa = new SimulatedAnnealing(saParameters);
            ParallelMultistart<LocalDescent, LocalDescentParameters> pld = new ParallelMultistart<LocalDescent, LocalDescentParameters>(ldParameters, multistartOptions);
            ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters> psa = new ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters>(saParameters, multistartOptions);
            IOptimizationAlgorithm optimizer = sa;
            ISolution bestSolution = solution;
            foreach (ISolution s in optimizer.Minimize(solution))
            {
                //if (s.IsCurrentBest) Console.WriteLine("{0}, {1:f}s, {2}, {3}, {4}, {5}, {6}", s.CostValue, s.TimeInSeconds, s.IterationNumber, s.IsCurrentBest, s.IsFinal, bestSolution.CostValue - s.CostValue, optimizer.SearchHistory?.Count());
                bestSolution = s;
            }

            Console.WriteLine(bestSolution.TimeInSeconds + "s");

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

            Console.ReadLine();
        }


        static private IEnumerable<Bitmap> DrawSolution(ISolution solution, IOptimizationAlgorithm optimizer)
        {
            yield return (solution as TspSolution)?.Draw();
            yield return (solution as FloorplanSolution)?.Draw();
            yield return DrawCostDiagram.Draw(optimizer, new BitmapStyle());
        }
    }
}