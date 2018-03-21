using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using LocalSearchOptimization.Examples.Problems.TravellingSalesman;
using LocalSearchOptimization.Examples.Problems.VehicleRouting;
using LocalSearchOptimization.Parameters;
using LocalSearchOptimization.Examples.RectangularPacking;
using LocalSearchOptimization.Examples;

namespace LocalSearchOptimizationGUI
{
    public partial class LocalSearchForm : Form
    {
        private bool solveTsp = true;
        private bool solveVehicleRouting = false;
        private bool solveFloorplan = true;

        private int tspCities = 225;

        private int routingCustomers = 225;
        private int routingVehicles = 25;

        private int floorplanRectangles = 70;

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

        private Bitmap leftSolutionImage;
        private Bitmap leftCostImage;
        private Bitmap rightSolutionImage;
        private Bitmap rightCostImage;

        private int optimizerType = 0;

        private bool isLeftPanelActive = true;
        private bool isRightPanelActive = true;
        private bool isSolutionDraw = true;
        private bool isCostDraw = true;

        private bool toRenderBackground = true;
        

        public LocalSearchForm()
        {
            solutionStyle = new BitmapStyle(style);
            costStyle = new BitmapStyle(style);

            bwTsp.DoWork += new DoWorkEventHandler(bwTsp_DoWork);
            bwTsp.ProgressChanged += new ProgressChangedEventHandler(bwTsp_ProgressChanged);
            bwTsp.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            bwVehicleRouting.DoWork += new DoWorkEventHandler(bwVehicleRouting_DoWork);
            bwVehicleRouting.ProgressChanged += new ProgressChangedEventHandler(bwVehicleRouting_ProgressChanged);
            bwVehicleRouting.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            bwFloorplan.DoWork += new DoWorkEventHandler(bwFloorplan_DoWork);
            bwFloorplan.ProgressChanged += new ProgressChangedEventHandler(bwFloorplan_ProgressChanged);
            bwFloorplan.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            InitializeComponent();

            GetInitialSolutions();
        }

        private void GetInitialSolutions()
        {
            tspSolution = new TspSolution(new TspProblem(tspCities));
            vehicleRoutingSolution = new VehicleRoutingSolution(new VehicleRoutingProblem(routingCustomers, routingVehicles));
            floorplanSolution = new FloorplanSolution(new FloorplanProblem(floorplanRectangles)).Shuffle(1) as FloorplanSolution;
            for (int i = 0; i < 12; i++)
            {
                floorplanSolution = floorplanSolution.Transcode() as FloorplanSolution;
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(bwTsp.IsBusy || bwVehicleRouting.IsBusy || bwFloorplan.IsBusy))
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
            if (!bwTsp.IsBusy && solveTsp) bwTsp.RunWorkerAsync();
            if (!bwVehicleRouting.IsBusy && solveVehicleRouting) bwVehicleRouting.RunWorkerAsync();
            if (!bwFloorplan.IsBusy && solveFloorplan) bwFloorplan.RunWorkerAsync();
        }

        private void LocalSearchForm_Paint(object sender, PaintEventArgs e)
        {
            if (leftSolutionImage != null && isLeftPanelActive && isSolutionDraw) e.Graphics.DrawImage(leftSolutionImage, 0, menuBar.Height);
            if (leftCostImage != null && isLeftPanelActive && isCostDraw) e.Graphics.DrawImage(leftCostImage, 0, menuBar.Height + (isSolutionDraw ? solutionStyle.ImageHeight : 0));
            if (rightSolutionImage != null && isRightPanelActive && isSolutionDraw) e.Graphics.DrawImage(rightSolutionImage, isLeftPanelActive ? solutionStyle.ImageWidth : 0, menuBar.Height);
            if (rightCostImage != null && isRightPanelActive && isCostDraw) e.Graphics.DrawImage(rightCostImage, isLeftPanelActive ? solutionStyle.ImageWidth : 0, menuBar.Height + (isSolutionDraw ? solutionStyle.ImageHeight : 0));
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (toRenderBackground) base.OnPaintBackground(e);
        }

        private void LocalSearchForm_Resize(object sender, EventArgs e)
        {
            int imageWidth = Math.Max(Width / ((isLeftPanelActive ? 1 : 0) + (isRightPanelActive ? 1 : 0)) - 8, 10);
            int imageHeight = Math.Max((Height - 2 * (menuBar.Height + statusBar.Height) + 8) / (isSolutionDraw && isCostDraw ? 3 : 1), 10);

            solutionStyle.ImageWidth = imageWidth;
            solutionStyle.ImageHeight = imageHeight * (isSolutionDraw && isCostDraw ? 2 : 1);

            costStyle.ImageWidth = imageWidth;
            costStyle.ImageHeight = imageHeight;

            if (solveTsp)
            {
                DrawTspSolutionAsync();
                DrawTspCostAsync();
            }
            if (solveVehicleRouting)
            {
                DrawVehicleRoutingSolutionAsync();
                DrawVehicleRoutingCostAsync();
            }
            if (solveFloorplan)
            {
                DrawFloorplanSolutionAsync();
                DrawFloorplanCostAsync();
            }

            Invalidate();
        }

        private void LocalSearchForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                bwTsp.CancelAsync();
                bwVehicleRouting.CancelAsync();
                bwFloorplan.CancelAsync();
            }
            else if (e.KeyCode == Keys.Left)
            {
                isLeftPanelActive = isRightPanelActive ? false : true;
                isRightPanelActive = true;
                LocalSearchForm_Resize(sender, e);
            }
            else if (e.KeyCode == Keys.Right)
            {
                isRightPanelActive = isLeftPanelActive ? false : true;
                isLeftPanelActive = true;
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
