using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples;
using LocalSearchOptimization.Examples.Problems.TravellingSalesman;
using LocalSearchOptimization.Examples.Structures.Permutation;
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
        private TspSolution tspSolution;

        private IOptimizationAlgorithm tspOptimizer;

        private BackgroundWorker bwTsp = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };

        Task tspSolutionDrawTask;
        Task tspCostDrawTask;


        private void bwTsp_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            algorithmStatus.Text = "Press ESC to cancel";

            TspProblem problem = new TspProblem(tspCities);
            TspSolution startSolution = new TspSolution(problem);

            Swap swap = new Swap(problem.Dimension, 1);
            Shift shift = new Shift(problem.Dimension, 2);
            TwoOpt twoOpt = new TwoOpt(problem.Dimension, 3);

            List<Operator> operations = new List<Operator> { swap, shift, twoOpt };
            MultistartParameters multistartParameters = (MultistartParameters)multistartOptions.Clone();

            LocalDescentParameters ldParameters = new LocalDescentParameters()
            {
                Name = "TSP LD",
                Seed = seed,
                DetailedOutput = true,
                Operators = operations
            };

            SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters()
            {
                Name = "TSP SA",
                InitProbability = 0.5,
                TemperatureCooling = 0.91,
                MinCostDeviation = 10E-3,
                Seed = seed,
                DetailedOutput = true,
                Operators = operations
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
                        saParameters.InitProbability = 0.01;
                        saParameters.MinCostDeviation = 10E-2;
                        multistartParameters.Parameters = ssParameters;
                        multistartParameters.OptimizationAlgorithm = typeof(StackedSearch);
                    }
                    break;
                case 3:
                    {
                        saParameters.InitProbability = 0.01;
                        saParameters.MinCostDeviation = 10E-2;
                        multistartParameters.InstancesNumber = 3;
                        multistartParameters.Parameters = ssParameters;
                        multistartParameters.OptimizationAlgorithm = typeof(StackedSearch);
                    }
                    break;
            }

            tspOptimizer = new ParallelMultistart(multistartParameters);

            toRenderBackground = false;

            foreach (ISolution solution in tspOptimizer.Minimize(startSolution))
            {
                if (worker.CancellationPending)
                {
                    tspOptimizer.Stop();
                    e.Cancel = true;
                }
                if (e.Cancel) solution.IsFinal = false;
                worker.ReportProgress(0);
            }
        }

        private void bwTsp_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!isLeftPanelActive) return;
            if (isSolutionDraw) DrawTspSolutionAsync();
            if (isCostDraw) DrawTspCostAsync();
        }

        private void DrawTspSolutionAsync()
        {
            if (tspOptimizer?.CurrentSolution?.IsFinal ?? false) tspSolutionDrawTask.Wait();
            if (tspSolutionDrawTask?.IsCompleted ?? true)
                tspSolutionDrawTask = Task.Factory.StartNew(() =>
                {
                    leftSolutionImage = ((tspOptimizer?.CurrentSolution ?? tspSolution) as TspSolution)?.Draw(solutionStyle);
                    Invalidate();
                });
        }

        private void DrawTspCostAsync()
        {
            if (tspOptimizer?.CurrentSolution?.IsFinal ?? false) tspCostDrawTask.Wait();
            if (tspCostDrawTask?.IsCompleted ?? true)
                tspCostDrawTask = Task.Factory.StartNew(() =>
                {
                    leftCostImage = DrawCostDiagram.Draw(tspOptimizer, costStyle, 30000);
                    Invalidate();
                });
        }
    }
}
