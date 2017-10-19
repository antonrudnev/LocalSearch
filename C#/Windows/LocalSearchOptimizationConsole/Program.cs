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


            MultistartParameters multistartOptions = new MultistartParameters()
            {
                Name = "P",
                InstancesNumber = 5,
                RandomizeStart = false,
                DetailedOutput = true,
                OutputFrequency = 100,
            };

            LocalDescentParameters ldParameters = new LocalDescentParameters()
            {
                Name = "LD",
                DetailedOutput = true,
                Seed = 0,
                Operators = operations,
                IsSteepestDescent = false
            };

            SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters()
            {
                Name = "SA",
                InitProbability = 0.01,
                TemperatureCooling = 0.94,
                MinCostDeviation = 10E-5,
                UseWeightedNeighborhood = false,
                DetailedOutput = true,
                Seed = 0,
                Operators = operations,
            };

            MultistartParameters pldParameters = (MultistartParameters)multistartOptions.Clone();
            pldParameters.OptimizationAlgorithm = typeof(LocalDescent);
            pldParameters.Parameters = ldParameters;

            MultistartParameters psaParameters = (MultistartParameters)multistartOptions.Clone();
            psaParameters.OptimizationAlgorithm = typeof(SimulatedAnnealing);
            psaParameters.Parameters = saParameters;

            StackedParameters ssParameters = new StackedParameters()
            {
                Name = "B",
                DetailedOutput = true,
                OptimizationAlgorithms = new Type[] { typeof(LocalDescent), typeof(SimulatedAnnealing), typeof(LocalDescent) },
                Parameters = new OptimizationParameters[] { ldParameters, saParameters, ldParameters }
            };

            StackedParameters sspParameters = new StackedParameters()
            {
                Name = "B",
                DetailedOutput = false,
                OptimizationAlgorithms = new Type[] { typeof(LocalDescent), typeof(ParallelMultistart), typeof(LocalDescent) },
                Parameters = new OptimizationParameters[] { ldParameters, psaParameters, ldParameters }
            };



            LocalDescent ld = new LocalDescent(ldParameters);
            SimulatedAnnealing sa = new SimulatedAnnealing(saParameters);
            ParallelMultistart pld = new ParallelMultistart(pldParameters);
            ParallelMultistart psa = new ParallelMultistart(psaParameters);

            StackedSearch ss = new StackedSearch(ssParameters);
            StackedSearch ssp = new StackedSearch(sspParameters);


            MultistartParameters pssParameters = (MultistartParameters)multistartOptions.Clone();
            pssParameters.DetailedOutput = false;
            pssParameters.RandomizeStart = true;
            pssParameters.OptimizationAlgorithm = typeof(StackedSearch);
            pssParameters.Parameters = ssParameters;

            ParallelMultistart pss = new ParallelMultistart(pssParameters);

            IOptimizationAlgorithm optimizer = pss;
            ISolution bestSolution = solution;
            foreach (ISolution s in optimizer.Minimize(solution))
            {
                //if (s.IsCurrentBest)
                    Console.WriteLine("\t{0}, {1:f}s, {2}, {3}, {4}, {5}, {6}", s.CostValue, s.TimeInSeconds, s.IterationNumber, s.IsCurrentBest, s.IsFinal, bestSolution.CostValue - s.CostValue, s.InstanceTag);
                bestSolution = s;
            }

            Console.WriteLine(bestSolution.TimeInSeconds + "s");
            Console.WriteLine(bestSolution.IterationNumber + " iterations");

            //var groups = optimizer.SearchHistory.GroupBy(s => s.OperatorTag).Select(s => new { Operator = s.Key, Count = s.Count() });
            //var dictionary = groups.ToDictionary(g => g.Operator, g => g.Count);

            //foreach (var o in groups)
            //{
            //    Console.WriteLine("{0} = {1}", o.Operator, o.Count);
            //}

            //Console.WriteLine("Done");


            foreach (var b in DrawSolution(bestSolution, optimizer))
            {
                b?.Save("solution" + DateTime.Now.Millisecond + ".jpg");
            }

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