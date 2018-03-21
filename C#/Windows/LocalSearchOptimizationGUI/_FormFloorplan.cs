using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples;
using LocalSearchOptimization.Examples.RectangularPacking;
using LocalSearchOptimization.Examples.Structures.Permutation;
using LocalSearchOptimization.Examples.Structures.Tree;
using LocalSearchOptimization.Parameters;
using LocalSearchOptimization.Solvers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalSearchOptimizationGUI
{
    public partial class LocalSearchForm : Form
    {
        private FloorplanSolution floorplanSolution;

        private IOptimizationAlgorithm floorplanOptimizer;

        private BackgroundWorker bwFloorplan = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };

        Task floorplanSolutionDrawTask;
        Task floorplanCostDrawTask;


        private void bwFloorplan_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            algorithmStatus.Text = "Press ESC to cancel";

            FloorplanProblem problem = new FloorplanProblem(floorplanRectangles);
            FloorplanSolution startSolution = new FloorplanSolution(problem);

            Swap swap = new Swap(problem.Dimension, 10);
            Shift shift = new Shift(problem.Dimension, 1);
            EmptyLeafMove eLeaf = new EmptyLeafMove(problem.Dimension, 5);
            FullLeafMove fLeaf = new FullLeafMove(problem.Dimension, 5);
            FullNodeMove fNode = new FullNodeMove(problem.Dimension, 5);

            MultistartParameters multistartParameters = (MultistartParameters)multistartOptions.Clone();

            LocalDescentParameters ldParameters = new LocalDescentParameters()
            {
                Name = "VLSI LD",
                Seed = seed,
                DetailedOutput = true,
                Operators = new List<Operator> { swap, fLeaf }
            };

            SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters()
            {
                Name = "VLSI SA",
                InitProbability = 0.5,
                TemperatureCooling = 0.96,
                MinCostDeviation = 0,
                Seed = seed,
                DetailedOutput = true,
                Operators = new List<Operator> { swap, fNode }
            };

            StackedParameters ssParameters = new StackedParameters()
            {
                Name = "B",
                DetailedOutput = true,
                OptimizationAlgorithms = new Type[] { typeof(LocalDescent), typeof(SimulatedAnnealing), typeof(LocalDescent) },
                Parameters = new OptimizationParameters[] { ldParameters, saParameters, ldParameters }
            };

            switch (optimizerType)
            {
                case 0:
                    {
                        multistartParameters.Parameters = ldParameters;
                        multistartParameters.OptimizationAlgorithm = typeof(LocalDescent);
                    }
                    break;
                case 1:
                    {
                        multistartParameters.Parameters = saParameters;
                        multistartParameters.OptimizationAlgorithm = typeof(SimulatedAnnealing);
                    }
                    break;
                case 2:
                    {
                        saParameters.InitProbability = 0.005;
                        saParameters.TemperatureCooling = 0.95;
                        multistartParameters.Parameters = ssParameters;
                        multistartParameters.OptimizationAlgorithm = typeof(StackedSearch);
                    }
                    break;
                case 3:
                    {
                        saParameters.InitProbability = 0.005;
                        saParameters.TemperatureCooling = 0.95;
                        multistartParameters.InstancesNumber = 3;
                        multistartParameters.Parameters = ssParameters;
                        multistartParameters.OptimizationAlgorithm = typeof(StackedSearch);
                    }
                    break;
            }

            floorplanOptimizer = new ParallelMultistart(multistartParameters);

            toRenderBackground = false;

            foreach (ISolution solution in floorplanOptimizer.Minimize(startSolution))
            {
                if (worker.CancellationPending)
                {
                    floorplanOptimizer.Stop();
                    e.Cancel = true;
                }
                worker.ReportProgress(0);
            }
        }

        private void bwFloorplan_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!isRightPanelActive) return;
            if (isSolutionDraw) DrawFloorplanSolutionAsync();
            if (isCostDraw) DrawFloorplanCostAsync();
        }

        private void DrawFloorplanSolutionAsync()
        {
            if (floorplanOptimizer?.CurrentSolution?.IsFinal ?? false) floorplanSolutionDrawTask.Wait();
            if (floorplanSolutionDrawTask?.IsCompleted ?? true)
                floorplanSolutionDrawTask = Task.Factory.StartNew(() =>
                {
                    rightSolutionImage = ((floorplanOptimizer?.CurrentSolution ?? floorplanSolution) as FloorplanSolution)?.Draw(solutionStyle);
                    Invalidate();
                });
        }

        private void DrawFloorplanCostAsync()
        {
            if (floorplanOptimizer?.CurrentSolution?.IsFinal ?? false) floorplanCostDrawTask.Wait();
            if (floorplanCostDrawTask?.IsCompleted ?? true)
                floorplanCostDrawTask = Task.Factory.StartNew(() =>
                {
                    rightCostImage = DrawCostDiagram.Draw(floorplanOptimizer, costStyle, 30000);
                    Invalidate();
                });
        }
    }
}
