using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples;
using LocalSearchOptimization.Examples.Problems.VehicleRouting;
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
        private VehicleRoutingSolution vehicleRoutingSolution;

        private IOptimizationAlgorithm vehicleRoutingOptimizer;

        private BackgroundWorker bwVehicleRouting = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };

        Task vehicleRoutingSolutionDrawTask;
        Task vehicleRoutingCostDrawTask;


        private void bwVehicleRouting_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            algorithmStatus.Text = "Press ESC to cancel";

            VehicleRoutingProblem problem = new VehicleRoutingProblem(routingCustomers, routingVehicles);
            VehicleRoutingSolution startSolution = new VehicleRoutingSolution(problem);

            Swap swap = new Swap(problem.Dimension, 1);
            Shift shift = new Shift(problem.Dimension, 2);
            TwoOpt twoOpt = new TwoOpt(problem.Dimension, 3);

            List<Operator> operations = new List<Operator> { swap, shift, twoOpt };
            MultistartParameters multistartParameters = (MultistartParameters)multistartOptions.Clone();

            LocalDescentParameters ldParameters = new LocalDescentParameters()
            {
                Name = "VEHICLE LD",
                Seed = seed,
                DetailedOutput = true,
                Operators = operations
            };

            SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters()
            {
                Name = "VEHICLE SA",
                InitProbability = 0.4,
                TemperatureCooling = 0.95,
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
                        saParameters.InitProbability = 0.002;
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

            vehicleRoutingOptimizer = new ParallelMultistart(multistartParameters);

            toRenderBackground = false;

            foreach (ISolution solution in vehicleRoutingOptimizer.Minimize(startSolution))
            {
                if (worker.CancellationPending)
                {
                    vehicleRoutingOptimizer.Stop();
                    e.Cancel = true;
                }
                if (e.Cancel) solution.IsFinal = false;
                worker.ReportProgress(0);
            }
        }

        private void bwVehicleRouting_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!isRightPanelActive) return;
            if (isSolutionDraw) DrawVehicleRoutingSolutionAsync();
            if (isCostDraw) DrawVehicleRoutingCostAsync();
        }

        private void DrawVehicleRoutingSolutionAsync()
        {
            if (vehicleRoutingOptimizer?.CurrentSolution?.IsFinal ?? false) vehicleRoutingSolutionDrawTask.Wait();
            if (vehicleRoutingSolutionDrawTask?.IsCompleted ?? true)
                vehicleRoutingSolutionDrawTask = Task.Factory.StartNew(() =>
                {
                    rightSolutionImage = ((vehicleRoutingOptimizer?.CurrentSolution ?? vehicleRoutingSolution) as VehicleRoutingSolution)?.Draw(solutionStyle);
                    Invalidate();
                });
        }

        private void DrawVehicleRoutingCostAsync()
        {
            if (vehicleRoutingOptimizer?.CurrentSolution?.IsFinal ?? false) vehicleRoutingCostDrawTask.Wait();
            if (vehicleRoutingCostDrawTask?.IsCompleted ?? true)
                vehicleRoutingCostDrawTask = Task.Factory.StartNew(() =>
                {
                    rightCostImage = DrawCostDiagram.Draw(vehicleRoutingOptimizer, costStyle, 30000);
                    Invalidate();
                });
        }
    }
}
