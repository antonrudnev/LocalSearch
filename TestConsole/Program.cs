using LocalSearch.Solver;
using LocalSearch.Packing2D;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalSearch
{
    class Program
    {
        static void Main(string[] args)
        {

            Packing2DProblem probblem = new Packing2DProblem(50);
            Packing2DSolution packing2DSolution = new Packing2DSolution(probblem);
            

            Packing2DNeighbourhood neighbourhood = new Packing2DNeighbourhood(probblem);

            //SimulatedAnnealing simulatedAnnealing = new SimulatedAnnealing(tspSwap);
            //foreach (SolutionDetails c in simulatedAnnealing.Minimize(tempLevelIterations:100000,isFrozenIterations: 100000))
            //{
            //    if (c.IsBest) Console.WriteLine(c.CostValue);
            //}

            List<string> operators = new List<string>();

            LocalDescentSearch localDescentSearch = new LocalDescentSearch(neighbourhood);
            foreach (SolutionDetails s in localDescentSearch.Minimize(5))
            {
                Console.WriteLine(s.CostValue);
                operators.Add(s.N);
            }

            var groups = operators.GroupBy(s => s).Select(s => new { Operator = s.Key, Count = s.Count() });
            var dictionary = groups.ToDictionary(g => g.Operator, g => g.Count);

            foreach (var o in groups)
            {
                Console.WriteLine("{0} = {1}", o.Operator, o.Count);
            }

            Console.ReadLine();
        }
    }
}