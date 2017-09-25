using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using LocalSearchOptimization.Examples.Problems.TravelingSalesman;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples.Structures.Permutation;
using LocalSearchOptimization.Solvers;
using LocalSearchOptimization.Parameters;
using LocalSearchOptimization.Examples.RectangularPacking;
using LocalSearchOptimization.Examples.Structures.Tree;
using LocalSearchOptimization.Examples.Problems;

namespace TestForm
{
    public partial class LocalSearchForm : Form
    {
        FloorplanProblem problem = new FloorplanProblem(20);

        private int multistart = 10;

        private int layoutW = 300;
        private int layoutH = 300;
        private int topMargin = 24;
        private List<double> saCost = new List<double>();
        private List<double> ldCost = new List<double>();
        private double saMinCost;
        private double saMaxCost;
        private double ldMinCost;
        private double ldMaxCost;
        private double saTime;
        private double ldTime;
        private Bitmap saBestLayout;
        private Bitmap ldBestLayout;
        private Bitmap saCurrentLayout;
        private Bitmap ldCurrentLayout;
        private bool wasResized;
        private bool toRenderBackground = true;
        private List<Bitmap> layouts = new List<Bitmap>();

        public LocalSearchForm()
        {
            InitializeComponent();
            LayoutsInit();
        }

        private void LayoutsInit()
        {
            layoutW = (this.Width - 20) / 2;
            layoutH = (this.Height - 84) / 2;
            saBestLayout = new Bitmap(layoutW, layoutH, PixelFormat.Format32bppRgb);
            ldBestLayout = new Bitmap(layoutW, layoutH, PixelFormat.Format32bppRgb);
            saCurrentLayout = new Bitmap(layoutW, layoutH, PixelFormat.Format32bppRgb);
            ldCurrentLayout = new Bitmap(layoutW, layoutH, PixelFormat.Format32bppRgb);
            layouts.Clear();
            layouts.AddRange(new List<Bitmap> { ldCurrentLayout, ldBestLayout, saCurrentLayout, saBestLayout });
            foreach (var l in layouts)
                using (Graphics g = Graphics.FromImage(l))
                {
                    g.Clear(SystemColors.Control);
                }
        }

        private void simulatedAnnealingMenuItem_Click(object sender, EventArgs e)
        {
            costValueStatus.Text = "";
            iterationStatus.Text = "";
            algorithmStatus.Text = "Running...";

            FloorplanSolution startSolution = new FloorplanSolution(problem);

            Swap swap = new Swap(problem.Dimension);
            Shift shift = new Shift(problem.Dimension);
            TwoOpt twoOpt = new TwoOpt(problem.Dimension);
            Leaf leaf = new Leaf(problem.Dimension);

            MultistartOptions multistartOptions = new MultistartOptions()
            {
                InstancesNumber = 10,
                OutputFrequency = 100,
                ReturnImprovedOnly = false
            };

            SimulatedAnnealingParameters saParameters = new SimulatedAnnealingParameters()
            {
                InitProbability = 0.1,
                TemperatureCooling = 0.97,
                MaxPassesSinceLastTransition = 0.5,
                UseWeightedNeighborhood = false,
                DetailedOutput = false,
                Seed = 0,
                Operators = new List<Operator> { swap, shift, leaf }
            };

            SimulatedAnnealing simulatedAnnealing = new SimulatedAnnealing(saParameters);

            if (wasResized)
            {
                wasResized = false;
                LayoutsInit();
            }

            layouts.Clear();
            layouts.AddRange(new List<Bitmap> { saCurrentLayout, saBestLayout, ldCurrentLayout, ldBestLayout });
            saCost.Clear();
            saMinCost = int.MaxValue;
            saMaxCost = 0;

            foreach (ISolution solution in simulatedAnnealing.Minimize(startSolution))
            {
                DrawSolution((IGeometricalSolution)solution, solution.IsCurrentBest ? saBestLayout : saCurrentLayout, "SA");
                saCost.Add(solution.CostValue);
                if (solution.CostValue > saMaxCost) saMaxCost = solution.CostValue;
                if (solution.CostValue < saMinCost) saMinCost = solution.CostValue;
                saTime = solution.TimeInSeconds;
            }

            algorithmStatus.Text = "Done";
        }

        private void localDescentMenuItem_Click(object sender, EventArgs e)
        {
            costValueStatus.Text = "";
            iterationStatus.Text = "";
            algorithmStatus.Text = "Running...";

            FloorplanSolution startSolution = new FloorplanSolution(problem);

            Swap swap = new Swap(problem.Dimension);
            Shift shift = new Shift(problem.Dimension);
            TwoOpt twoOpt = new TwoOpt(problem.Dimension);
            Leaf leaf = new Leaf(problem.Dimension);

            MultistartOptions multistartOptions = new MultistartOptions()
            {
                InstancesNumber = 10,
                OutputFrequency = 100,
                ReturnImprovedOnly = true
            };

            LocalDescentParameters ldParameters = new LocalDescentParameters()
            {
                DetailedOutput = true,
                Seed = 0,
                Operators = new List<Operator> { swap, shift, leaf },
                IsSteepestDescent = false
            };

            LocalDescent localDescent = new LocalDescent(ldParameters);

            if (wasResized)
            {
                wasResized = false;
                LayoutsInit();
            }

            layouts.Clear();
            layouts.AddRange(new List<Bitmap> { ldCurrentLayout, ldBestLayout, saCurrentLayout, saBestLayout });
            ldCost.Clear();
            ldMinCost = int.MaxValue;
            ldMaxCost = 0;

            foreach (ISolution solution in localDescent.Minimize(startSolution))
            {
                DrawSolution((IGeometricalSolution)solution, solution.IsCurrentBest ? ldBestLayout : ldCurrentLayout, "LD");
                ldCost.Add(solution.CostValue);
                if (solution.CostValue > ldMaxCost) ldMaxCost = solution.CostValue;
                if (solution.CostValue < ldMinCost) ldMinCost = solution.CostValue;
                ldTime = solution.TimeInSeconds;
            }

            algorithmStatus.Text = "Done";
        }

        private void DrawSolution(IGeometricalSolution solution, Bitmap bitmap, string alg)
        {
            toRenderBackground = false;
            if (solution.IsCurrentBest)
            {
                costValueStatus.Text = solution.CostValue.ToString() + "  " + solution.DerivedByOperation;
                iterationStatus.Text = "It." + solution.IterationNumber.ToString();
            }
            double scaleX = (bitmap.Width) / solution.Details.MaxWidth;
            double scaleY = (bitmap.Height - 2) / solution.Details.MaxHeight;

            int dotRadius = 5;

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Pen pen = solution.IsCurrentBest && alg == "SA" ? new Pen(Color.Green) : alg == "SA" ? new Pen(Color.Blue) : solution.IsCurrentBest ? new Pen(Color.Red) : new Pen(Color.Purple);
                SolidBrush brush = solution.IsCurrentBest && alg == "SA" ? new SolidBrush(Color.LightGreen) : alg == "SA" ? new SolidBrush(Color.LightBlue) : solution.IsCurrentBest ? new SolidBrush(Color.Orange) : new SolidBrush(Color.Magenta); ;
                g.Clear(SystemColors.Control);
                foreach (var line in solution.Details.Lines)
                {
                    g.DrawLine(pen, (float)(line.Item1 * scaleX), (float)(line.Item2 * scaleY), (float)(line.Item3 * scaleX), (float)(line.Item4 * scaleY));
                    g.FillEllipse(brush, (float)(line.Item1 * scaleX - dotRadius), (float)(line.Item2 * scaleY - dotRadius), 2 * dotRadius, 2 * dotRadius);
                }
                foreach (var rect in solution.Details.Rectangles)
                {
                    float x = (float)(rect.Item1 * scaleX);
                    float y = (float)(bitmap.Height - (rect.Item2 + rect.Item4) * scaleY) - 1;
                    float w = (float)(rect.Item3 * scaleX);
                    float h = (float)(rect.Item4 * scaleY) - 1;
                    g.FillRectangle(brush, x, y, w, h);
                    g.DrawRectangle(pen, x, y, w, h);
                }
                g.DrawString(alg + (solution.IsCurrentBest ? " best: " : " current: ") + Math.Round(solution.CostValue, 4) + ", It." + solution.IterationNumber.ToString() + ", " + Math.Round(solution.TimeInSeconds, 3).ToString() + "s", new Font(SystemFonts.DefaultFont, FontStyle.Bold), new SolidBrush(Color.Black), 0, 0);
            }
            Application.DoEvents();
            this.Invalidate();
            toRenderBackground = true;
        }

        private void LocalSearchForm_Paint(object sender, PaintEventArgs e)
        {
            if (!displayCostMenuItem.Checked)
            {
                for (int i = 0; i < layouts.Count; i++)
                {
                    if (!(layouts[i] is null))
                    {
                        e.Graphics.DrawImage(layouts[i], (i % 2) * layoutW, (i / 2) * layoutH + topMargin);
                    }
                }
            }
            else
            {
                int dotRadius = 2;
                double minCost = Math.Min(saMinCost, ldMinCost);
                double maxCost = Math.Max(saMaxCost, ldMaxCost);
                int lW = (this.Width - 20);
                int lH = (this.Height - 84) / 2;
                
                Bitmap saCostLayout = new Bitmap(lW, lH, PixelFormat.Format32bppRgb);
                Bitmap ldCostLayout = new Bitmap(lW, lH, PixelFormat.Format32bppRgb);

                double scaleX = (double)lW / (saCost.Count + 1);
                double scaleY = lH / (maxCost - minCost);

                using (Graphics g = Graphics.FromImage(saCostLayout))
                {
                    Pen pen = new Pen(Color.Blue);
                    SolidBrush brush = new SolidBrush(Color.Blue);
                    g.Clear(SystemColors.Control);
                    for (int i=0;i<saCost.Count;i++)
                    {
                        g.FillEllipse(brush, (float)(i * scaleX - dotRadius), lH - (float)(saCost[i] * scaleY - dotRadius), 2 * dotRadius, 2 * dotRadius);
                    }
                }

                scaleX = (double)lW / (ldCost.Count + 1);

                using (Graphics g = Graphics.FromImage(ldCostLayout))
                {
                    Pen pen = new Pen(Color.Blue);
                    SolidBrush brush = new SolidBrush(Color.Blue);
                    g.Clear(SystemColors.Control);
                    for (int i = 0; i < ldCost.Count; i++)
                    {
                        g.FillEllipse(brush, (float)(i * scaleX - dotRadius), lH - (float)(ldCost[i] * scaleY - dotRadius), 2 * dotRadius, 2 * dotRadius);
                    }
                }

                e.Graphics.DrawImage(saCostLayout, 0, topMargin);
                e.Graphics.DrawImage(ldCostLayout, 0, lH + topMargin);
            }
        }

        private void LocalSearchForm_SizeChanged(object sender, EventArgs e)
        {
            wasResized = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (toRenderBackground) base.OnPaintBackground(e);
        }

        private void dimesionTextBox_TextChanged(object sender, EventArgs e)
        {
            bool hasDimChanged = int.TryParse(dimesionTextBox.Text, out int newDim);
            if (hasDimChanged)
            {
                problem = new FloorplanProblem(newDim);
            }
        }

        private void multistartTextBox_TextChanged(object sender, EventArgs e)
        {
            bool hasMultistartChanged = int.TryParse(multistartTextBox.Text, out int newMultistart);
            if (hasMultistartChanged)
            {
                multistart = Math.Max(newMultistart, 1);
            }
        }

        private void displayCostMenuItem_Click(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void LocalSearchForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D)
            {
                displayCostMenuItem.Checked = !displayCostMenuItem.Checked;
            }
        }
    }
}