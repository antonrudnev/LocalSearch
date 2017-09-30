using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using LocalSearchOptimization.Examples.Problems.TravelingSalesman;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples.Structures.Permutation;
using LocalSearchOptimization.Solvers;
using LocalSearchOptimization.Parameters;
using LocalSearchOptimization.Examples.RectangularPacking;
using LocalSearchOptimization.Examples.Structures.Tree;
using LocalSearchOptimization.Examples;
using System.Threading.Tasks;

namespace LocalSearchOptimizationGUI
{
    public partial class LocalSearchForm : Form
    {
        private int tspDimension = 150;
        private int floorplanDimension = 70;

        private int seed = 0;

        private MultistartOptions multistartOptions = new MultistartOptions()
        {
            InstancesNumber = 3,
            OutputFrequency = 200,
            ReturnImprovedOnly = false
        };

        private BitmapStyle style = new BitmapStyle
        {
            MarginX = 0,
            MarginY = 50,
            FontName = "Microsoft Sans Serif",
            FontSize = 10,
            PenColor = "Green",
            PenWidth = 2,
            Radius = 4,
            BackgroundColor = SystemColors.Control.Name,
            CostRadius = 1
        };

        private Bitmap tspSolutionImage;
        private Bitmap tspCostImage;
        private Bitmap floorplanSolutionImage;
        private Bitmap floorplanCostImage;

        private IOptimizationAlgorithm tspOptimizer;
        private IOptimizationAlgorithm floorplanOptimizer;

        private BackgroundWorker bwTsp = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        private BackgroundWorker bwFloorplan = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        private bool stochasticOptimizer = true;
        private bool toRenderBackground = true;

        Task tspSolutionDrawTask;
        Task floorplanSolutionDrawTask;
        Task tspCostDrawTask;
        Task floorplanCostDrawTask;

        public LocalSearchForm()
        {
            InitializeComponent();

            bwTsp.DoWork += new DoWorkEventHandler(bwTsp_DoWork);
            bwTsp.ProgressChanged += new ProgressChangedEventHandler(bwTsp_ProgressChanged);
            bwTsp.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            bwFloorplan.DoWork += new DoWorkEventHandler(bwFloorplan_DoWork);
            bwFloorplan.ProgressChanged += new ProgressChangedEventHandler(bwFloorplan_ProgressChanged);
            bwFloorplan.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }


        private void bwTsp_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            this.algorithmStatus.Text = "Press ESC to cancel";

            TspProblem problem = new TspProblem(this.tspDimension);
            TspSolution startSolution = new TspSolution(problem);

            Swap swap = new Swap(problem.Dimension, 1);
            Shift shift = new Shift(problem.Dimension, 2);
            TwoOpt twoOpt = new TwoOpt(problem.Dimension, 3);

            List<Operator> operations = new List<Operator> { swap, shift, twoOpt };

            if (stochasticOptimizer)
            {
                SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters()
                {
                    Name = "TSP SA",
                    InitProbability = 0.3,
                    MinCostDeviation = 10E-5,
                    Seed = this.seed,
                    DetailedOutput = true,
                    Operators = operations,
                };
                tspOptimizer = new ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters>(saParameters, multistartOptions);
            }
            else
            {
                LocalDescentParameters ldParameters = new LocalDescentParameters()
                {
                    Name = "TSP LD",
                    Seed = this.seed,
                    DetailedOutput = true,
                    Operators = operations,
                };
                tspOptimizer = new ParallelMultistart<LocalDescent, LocalDescentParameters>(ldParameters, multistartOptions);
            }

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
            this.algorithmStatus.Text = "Press ESC to cancel";

            FloorplanProblem problem = new FloorplanProblem(this.floorplanDimension);
            FloorplanSolution startSolution = new FloorplanSolution(problem);

            Swap swap = new Swap(problem.Dimension, 10);
            Shift shift = new Shift(problem.Dimension, 1);
            Leaf leaf = new Leaf(problem.Dimension, 5);

            List<Operator> operations = new List<Operator> { swap, leaf };

            if (stochasticOptimizer)
            {
                SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters()
                {
                    Name = "VLSI SA",
                    InitProbability = 0.1,
                    TemperatureCooling = 0.97,
                    TemperatureLevelPower = 1,
                    MinCostDeviation = 0,
                    Seed = this.seed,
                    DetailedOutput = true,
                    Operators = operations,
                };
                floorplanOptimizer = new ParallelMultistart<SimulatedAnnealing, SimulatedAnnealingParameters>(saParameters, multistartOptions);
            }
            else
            {
                LocalDescentParameters ldParameters = new LocalDescentParameters()
                {
                    Name = "VLSI LD",
                    Seed = this.seed,
                    DetailedOutput = true,
                    Operators = operations,
                };
                floorplanOptimizer = new ParallelMultistart<LocalDescent, LocalDescentParameters>(ldParameters, multistartOptions);
            }

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
            DrawTspSolutionAsync();
            DrawTspCostAsync();
        }

        private void bwFloorplan_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DrawFloorplanSolutionAsync();
            DrawFloorplanCostAsync();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(bwTsp.IsBusy || bwFloorplan.IsBusy))
            {
                toRenderBackground = true;
                if (e.Cancelled == true)
                    this.algorithmStatus.Text = "Canceled";
                else if (e.Error != null)
                    this.algorithmStatus.Text = ("Error: " + e.Error.Message);
                else
                    this.algorithmStatus.Text = "Done";
            }
        }

        private void simulatedAnnealingMenuItem_Click(object sender, EventArgs e)
        {
            stochasticOptimizer = true;
            StartDemo();
        }

        private void localDescentMenuItem_Click(object sender, EventArgs e)
        {
            stochasticOptimizer = false;
            StartDemo();
        }

        private void StartDemo()
        {
            if (!bwTsp.IsBusy)
                bwTsp.RunWorkerAsync();
            if (!bwFloorplan.IsBusy)
                bwFloorplan.RunWorkerAsync();
        }

        private void LocalSearchForm_Paint(object sender, PaintEventArgs e)
        {
            if (tspSolutionImage != null) e.Graphics.DrawImage(tspSolutionImage, 0, menuBar.Height);
            if (tspCostImage != null) e.Graphics.DrawImage(tspCostImage, 0, menuBar.Height + style.ImageHeight);
            if (floorplanSolutionImage != null) e.Graphics.DrawImage(floorplanSolutionImage, style.ImageWidth, menuBar.Height);
            if (floorplanCostImage != null) e.Graphics.DrawImage(floorplanCostImage, style.ImageWidth, menuBar.Height + style.ImageHeight);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (toRenderBackground) base.OnPaintBackground(e);
        }

        private void LocalSearchForm_Resize(object sender, EventArgs e)
        {
            style.ImageWidth = Math.Max(this.Width / 2 - 8, 10);
            style.ImageHeight = Math.Max((this.Height - 2 * (menuBar.Height + statusBar.Height) + 8) / 2, 10);
            DrawTspSolutionAsync();
            DrawFloorplanSolutionAsync();
            DrawTspCostAsync();
            DrawFloorplanCostAsync();
            this.Invalidate();
        }

        private void DrawTspSolutionAsync()
        {
            if (tspOptimizer?.CurrentSolution?.IsFinal ?? false) tspSolutionDrawTask.Wait();
            if (tspSolutionDrawTask?.IsCompleted ?? true)
                tspSolutionDrawTask = Task.Factory.StartNew(() =>
                {
                    tspSolutionImage = (tspOptimizer?.CurrentSolution as TspSolution)?.Draw(style);
                    this.Invalidate();
                });
        }

        private void DrawFloorplanSolutionAsync()
        {
            if (floorplanOptimizer?.CurrentSolution?.IsFinal ?? false) floorplanSolutionDrawTask.Wait();
            if (floorplanSolutionDrawTask?.IsCompleted ?? true)
                floorplanSolutionDrawTask = Task.Factory.StartNew(() =>
                {
                    floorplanSolutionImage = (floorplanOptimizer?.CurrentSolution as FloorplanSolution)?.Draw(style);
                    this.Invalidate();
                });
        }

        private void DrawTspCostAsync()
        {
            if (tspOptimizer?.CurrentSolution?.IsFinal ?? false) tspCostDrawTask.Wait();
            if (tspCostDrawTask?.IsCompleted ?? true)
                tspCostDrawTask = Task.Factory.StartNew(() =>
                {
                    tspCostImage = DrawCostDiagram.Draw(tspOptimizer, style, 30000);
                    this.Invalidate();
                });
        }

        private void DrawFloorplanCostAsync()
        {
            if (floorplanOptimizer?.CurrentSolution?.IsFinal ?? false) floorplanCostDrawTask.Wait();
            if (floorplanCostDrawTask?.IsCompleted ?? true)
                floorplanCostDrawTask = Task.Factory.StartNew(() =>
                {
                    floorplanCostImage = DrawCostDiagram.Draw(floorplanOptimizer, style, 30000);
                    this.Invalidate();
                });
        }

        private void LocalSearchForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                bwTsp.CancelAsync();
                bwFloorplan.CancelAsync();
            }
        }
    }
}