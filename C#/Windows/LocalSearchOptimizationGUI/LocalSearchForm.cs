using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using LocalSearchOptimization.Examples.Problems.TravellingSalesman;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples.Structures.Permutation;
using LocalSearchOptimization.Solvers;
using LocalSearchOptimization.Parameters;
using LocalSearchOptimization.Examples.RectangularPacking;
using LocalSearchOptimization.Examples.Structures.Tree;
using LocalSearchOptimization.Examples;

namespace LocalSearchOptimizationGUI
{
    public partial class LocalSearchForm : Form
    {
        private int tspDimension = 225;
        private int floorplanDimension = 70;

        private int seed = 0;

        private MultistartParameters multistartOptions = new MultistartParameters()
        {
            InstancesNumber = 1,
            OutputFrequency = 200,
            DetailedOutput = true
        };

        private BitmapStyle style = new BitmapStyle
        {
            MarginX = 0,
            MarginY = 50,
            FontName = "Microsoft Sans Serif",
            FontSize = 10,
            BrushColor = "SpringGreen",
            PenColor = "DarkGreen",
            PenWidth = 2,
            Radius = 4,
            BackgroundColor = SystemColors.Control.Name,
        };

        private BitmapStyle solutionStyle;
        private BitmapStyle costStyle;

        private Bitmap tspSolutionImage;
        private Bitmap tspCostImage;
        private Bitmap floorplanSolutionImage;
        private Bitmap floorplanCostImage;

        private TspSolution tspSolution;
        private FloorplanSolution floorplanSolution;

        private IOptimizationAlgorithm tspOptimizer;
        private IOptimizationAlgorithm floorplanOptimizer;

        private BackgroundWorker bwTsp = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        private BackgroundWorker bwFloorplan = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        private int optimizerType = 0;
        private bool isTspActive = true;
        private bool isFloorplanActive = true;
        private bool isSolutionDraw = true;
        private bool isCostDraw = true;

        private bool toRenderBackground = true;

        Task tspSolutionDrawTask;
        Task floorplanSolutionDrawTask;
        Task tspCostDrawTask;
        Task floorplanCostDrawTask;

        public LocalSearchForm()
        {
            solutionStyle = new BitmapStyle(style);
            costStyle = new BitmapStyle(style);

            bwTsp.DoWork += new DoWorkEventHandler(bwTsp_DoWork);
            bwTsp.ProgressChanged += new ProgressChangedEventHandler(bwTsp_ProgressChanged);
            bwTsp.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            bwFloorplan.DoWork += new DoWorkEventHandler(bwFloorplan_DoWork);
            bwFloorplan.ProgressChanged += new ProgressChangedEventHandler(bwFloorplan_ProgressChanged);
            bwFloorplan.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            InitializeComponent();

            GetInitialSolutions();
        }

        private void GetInitialSolutions()
        {
            tspSolution = new TspSolution(new TspProblem(tspDimension));
            floorplanSolution = new FloorplanSolution(new FloorplanProblem(floorplanDimension)).Shuffle(1) as FloorplanSolution;
            for (int i = 0; i < 12; i++)
            {
                floorplanSolution = floorplanSolution.Transcode() as FloorplanSolution;
            }
        }

        private void bwTsp_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            algorithmStatus.Text = "Press ESC to cancel";

            TspProblem problem = new TspProblem(tspDimension);
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

        private void bwFloorplan_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            algorithmStatus.Text = "Press ESC to cancel";

            FloorplanProblem problem = new FloorplanProblem(floorplanDimension);
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

        private void bwTsp_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!isTspActive) return;
            if (isSolutionDraw) DrawTspSolutionAsync();
            if (isCostDraw) DrawTspCostAsync();
        }

        private void bwFloorplan_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!isFloorplanActive) return;
            if (isSolutionDraw) DrawFloorplanSolutionAsync();
            if (isCostDraw) DrawFloorplanCostAsync();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(bwTsp.IsBusy || bwFloorplan.IsBusy))
            {
                if (e.Cancelled == true)
                    algorithmStatus.Text = "Canceled";
                else if (e.Error != null)
                    algorithmStatus.Text = ("Error: " + e.Error.Message);
                else
                    algorithmStatus.Text = "Done";

                //toRenderBackground = true;
            }
        }

        private void localDescentMenuItem_Click(object sender, EventArgs e)
        {
            optimizerType = 0;
            StartDemo();
        }

        private void simulatedAnnealingMenuItem_Click(object sender, EventArgs e)
        {
            optimizerType = 1;
            StartDemo();
        }

        private void stackedSearchMenuItem_Click(object sender, EventArgs e)
        {
            optimizerType = 2;
            StartDemo();
        }

        private void parallelMultistartMenuItem_Click(object sender, EventArgs e)
        {
            optimizerType = 3;
            StartDemo();
        }

        private void StartDemo()
        {
            if (!bwTsp.IsBusy && isTspActive) bwTsp.RunWorkerAsync();
            if (!bwFloorplan.IsBusy && isFloorplanActive) bwFloorplan.RunWorkerAsync();
        }

        private void LocalSearchForm_Paint(object sender, PaintEventArgs e)
        {
            if (tspSolutionImage != null && isTspActive && isSolutionDraw) e.Graphics.DrawImage(tspSolutionImage, 0, menuBar.Height);
            if (tspCostImage != null && isTspActive && isCostDraw) e.Graphics.DrawImage(tspCostImage, 0, menuBar.Height + (isSolutionDraw ? solutionStyle.ImageHeight : 0));
            if (floorplanSolutionImage != null && isFloorplanActive && isSolutionDraw) e.Graphics.DrawImage(floorplanSolutionImage, isTspActive ? solutionStyle.ImageWidth : 0, menuBar.Height);
            if (floorplanCostImage != null && isFloorplanActive && isCostDraw) e.Graphics.DrawImage(floorplanCostImage, isTspActive ? solutionStyle.ImageWidth : 0, menuBar.Height + (isSolutionDraw ? solutionStyle.ImageHeight : 0));
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (toRenderBackground) base.OnPaintBackground(e);
        }

        private void LocalSearchForm_Resize(object sender, EventArgs e)
        {
            int imageWidth = Math.Max(Width / ((isTspActive ? 1 : 0) + (isFloorplanActive ? 1 : 0)) - 8, 10);
            int imageHeight = Math.Max((Height - 2 * (menuBar.Height + statusBar.Height) + 8) / (isSolutionDraw && isCostDraw ? 3 : 1), 10);

            solutionStyle.ImageWidth = imageWidth;
            solutionStyle.ImageHeight = imageHeight * (isSolutionDraw && isCostDraw ? 2 : 1);

            costStyle.ImageWidth = imageWidth;
            costStyle.ImageHeight = imageHeight;

            DrawTspSolutionAsync();
            DrawFloorplanSolutionAsync();
            DrawTspCostAsync();
            DrawFloorplanCostAsync();
            Invalidate();
        }

        private void DrawTspSolutionAsync()
        {
            if (tspOptimizer?.CurrentSolution?.IsFinal ?? false) tspSolutionDrawTask.Wait();
            if (tspSolutionDrawTask?.IsCompleted ?? true)
                tspSolutionDrawTask = Task.Factory.StartNew(() =>
                {
                    tspSolutionImage = ((tspOptimizer?.CurrentSolution ?? tspSolution) as TspSolution)?.Draw(solutionStyle);
                    Invalidate();
                });
        }

        private void DrawFloorplanSolutionAsync()
        {
            if (floorplanOptimizer?.CurrentSolution?.IsFinal ?? false) floorplanSolutionDrawTask.Wait();
            if (floorplanSolutionDrawTask?.IsCompleted ?? true)
                floorplanSolutionDrawTask = Task.Factory.StartNew(() =>
                {
                    floorplanSolutionImage = ((floorplanOptimizer?.CurrentSolution ?? floorplanSolution) as FloorplanSolution)?.Draw(solutionStyle);
                    Invalidate();
                });
        }

        private void DrawTspCostAsync()
        {
            if (tspOptimizer?.CurrentSolution?.IsFinal ?? false) tspCostDrawTask.Wait();
            if (tspCostDrawTask?.IsCompleted ?? true)
                tspCostDrawTask = Task.Factory.StartNew(() =>
                {
                    tspCostImage = DrawCostDiagram.Draw(tspOptimizer, costStyle, 30000);
                    Invalidate();
                });
        }

        private void DrawFloorplanCostAsync()
        {
            if (floorplanOptimizer?.CurrentSolution?.IsFinal ?? false) floorplanCostDrawTask.Wait();
            if (floorplanCostDrawTask?.IsCompleted ?? true)
                floorplanCostDrawTask = Task.Factory.StartNew(() =>
                {
                    floorplanCostImage = DrawCostDiagram.Draw(floorplanOptimizer, costStyle, 30000);
                    Invalidate();
                });
        }

        private void LocalSearchForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                bwTsp.CancelAsync();
                bwFloorplan.CancelAsync();
            }
            else if (e.KeyCode == Keys.Left)
            {
                isTspActive = isFloorplanActive ? false : true;
                isFloorplanActive = true;
                LocalSearchForm_Resize(sender, e);
            }
            else if (e.KeyCode == Keys.Right)
            {
                isFloorplanActive = isTspActive ? false : true;
                isTspActive = true;
                LocalSearchForm_Resize(sender, e);
            }
            else if (e.KeyCode == Keys.Up)
            {
                isSolutionDraw = isCostDraw ? false : true;
                isCostDraw = true;
                LocalSearchForm_Resize(sender, e);
            }
            else if (e.KeyCode == Keys.Down)
            {
                isCostDraw = isSolutionDraw ? false : true;
                isSolutionDraw = true;
                LocalSearchForm_Resize(sender, e);
            }
        }
    }
}