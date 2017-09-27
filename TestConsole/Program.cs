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
using TestConsole;

namespace LocalSearchOptimization
{
    class Program
    {      
        static void Main(string[] args)
        {


            //FloorplanProblem problem = new FloorplanProblem(50);
            //FloorplanSolution solution = new FloorplanSolution(problem);
            //Swap swap = new Swap(problem.Dimension);
            //Shift shift = new Shift(problem.Dimension);
            //Leaf leaf = new Leaf(problem.Dimension);
            //List<Operator> operations = new List<Operator> { swap, shift, leaf };




            TspProblem problem = new TspProblem(100);
            TspSolution solution = new TspSolution(problem);
            Swap swap = new Swap(problem.Dimension, 1);
            Shift shift = new Shift(problem.Dimension, 2);
            TwoOpt twoOpt = new TwoOpt(problem.Dimension, 3);
            List<Operator> operations = new List<Operator> { swap, shift, twoOpt };








            MultistartOptions multistartOptions = new MultistartOptions()
            {
                InstancesNumber = 2,
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
                MaxPassesSinceLastTransition = 0.05,
                UseWeightedNeighborhood = true,
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


            //var groups = operators.GroupBy(s => s).Select(s => new { Operator = s.Key, Count = s.Count() });
            //var dictionary = groups.ToDictionary(g => g.Operator, g => g.Count);

            //foreach (var o in groups)
            //{
            //    Console.WriteLine("{0} = {1}", o.Operator, o.Count);
            //}

            Console.WriteLine("Done");
            

            foreach(var b in DrawSolution(sol as IGeometricalSolution, new BitmapStyle()))
            {
                b.Save("solution" + DateTime.Now.Millisecond + ".jpg");
            }

            Console.ReadLine();
        }


        static private IEnumerable<Bitmap> DrawSolution(IGeometricalSolution solution, BitmapStyle style)
        {

            yield return (solution is TspSolution) ? DrawTravelingSalesman(solution, style) : (solution is FloorplanSolution) ? DrawFloorplan(solution, style) : null;
            yield return DrawCost(solution, style);
        }

        static private Bitmap DrawCost(ISolution solution, BitmapStyle style)
        {
            List<Color> colors = new List<Color> { Color.Blue, Color.Brown, Color.Cyan, Color.Green, Color.Purple, Color.Lime, Color.Magenta, Color.Maroon, Color.Navy, Color.Olive, Color.Orange, Color.Pink, Color.Purple, Color.Red, Color.Teal, Color.Yellow };

            Bitmap bitmap = new Bitmap(style.BitmapWidth, style.BitmapHeight, PixelFormat.Format32bppRgb);
            double minCost = solution.SolutionsHistory.Min(x => x.Item3);
            double maxCost = solution.SolutionsHistory.Max(x => x.Item3);
            double sX = bitmap.Width / (solution.SolutionsHistory.Count + 0.1);
            double sY = (bitmap.Height - 4 * style.CostRadius) / (maxCost - minCost);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(style.Background);
                for (int i = 0; i < solution.SolutionsHistory.Count - 1; i++)
                {
                    bool parsed = int.TryParse(solution.SolutionsHistory[i].Item1, out int instance);
                    g.FillEllipse(parsed ? new SolidBrush(colors[instance%colors.Count]) : style.CostBrush, (float)(i * sX - style.CostRadius), (float)(bitmap.Height - (solution.SolutionsHistory[i].Item3 - minCost) * sY - 5 * style.CostRadius), 2 * style.CostRadius, 2 * style.CostRadius);
                }
                //g.DrawString(String.Format("Tour lenght: {0}\nIteration: {1}\nTime: {2}s", Math.Round(solution.CostValue, 4), solution.IterationNumber, Math.Round(solution.TimeInSeconds, 3)), style.Font, new SolidBrush(Color.Black), 0, 0);
            }
            return bitmap;
        }


        static private Bitmap DrawTravelingSalesman(IGeometricalSolution solution, BitmapStyle style)
        {
            Bitmap bitmap = new Bitmap(style.BitmapWidth, style.BitmapHeight, PixelFormat.Format32bppRgb);
            double sX = bitmap.Width / solution.Details.MaxWidth;
            double sY = bitmap.Height / solution.Details.MaxHeight;
            PointF[] points = solution.Details.Points.Select(x => new PointF((float)(x.Item1 * sX), (float)(x.Item2 * sY))).ToArray();
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(style.Background);
                g.FillPolygon(style.Brush, points);
                g.DrawPolygon(style.Pen, points);
                points.ToList().ForEach(x =>
                {
                    g.FillEllipse(style.Brush, x.X - style.CityRadius, x.Y - style.CityRadius, 2 * style.CityRadius, 2 * style.CityRadius);
                    g.DrawEllipse(style.Pen, x.X - style.CityRadius, x.Y - style.CityRadius, 2 * style.CityRadius, 2 * style.CityRadius);
                });
                g.DrawString(String.Format("Tour lenght: {0}\nIteration: {1}\nTime: {2}s", Math.Round(solution.CostValue, 4), solution.IterationNumber, Math.Round(solution.TimeInSeconds, 3)), style.Font, new SolidBrush(Color.Black), 0, 0);
            }
            return bitmap;
        }

        static private Bitmap DrawFloorplan(IGeometricalSolution solution, BitmapStyle style)
        {
            Bitmap bitmap = new Bitmap(style.BitmapWidth, style.BitmapHeight, PixelFormat.Format32bppRgb);
            double sX = (bitmap.Width - 2 * style.Pen.Width) / solution.Details.MaxWidth;
            double sY = (bitmap.Height - 2 * style.Pen.Width) / solution.Details.MaxHeight;
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(style.Background);
                solution.Details.Rectangles.ForEach(r =>
                {
                    float x = (float)(r.Item1 * sX) + style.Pen.Width;
                    float y = (float)(bitmap.Height - (r.Item2 + r.Item4) * sY) - style.Pen.Width;
                    float w = (float)(r.Item3 * sX) + style.Pen.Width;
                    float h = (float)(r.Item4 * sY) - style.Pen.Width;
                    g.FillRectangle(style.Brush, x, y, w, h);
                    g.DrawRectangle(style.Pen, x, y, w, h);
                });
                g.DrawString(String.Format("Packing area: {0}\nIteration: {1}\nTime: {2}s", Math.Round(solution.CostValue, 4), solution.IterationNumber, Math.Round(solution.TimeInSeconds, 3)), style.Font, new SolidBrush(Color.Black), 0, 0);
            }
            return bitmap;
        }
    }
}