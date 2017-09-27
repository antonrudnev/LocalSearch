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
        TspProblem problem = new TspProblem(50);

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

            TspSolution startSolution = new TspSolution(problem);
            //FloorplanSolution startSolution = new FloorplanSolution(problem);

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
                Operators = new List<Operator> { swap, shift, twoOpt }
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

            TspSolution startSolution = new TspSolution(problem);
            //FloorplanSolution startSolution = new FloorplanSolution(problem);

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
                Operators = new List<Operator> { swap, shift, twoOpt },
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
                costValueStatus.Text = solution.CostValue.ToString() + "  " + solution.OperatorTag;
                iterationStatus.Text = "It." + solution.IterationNumber.ToString();
            }
            double sX = bitmap.Width / solution.Details.MaxWidth;
            double sY = (bitmap.Height - 2) / solution.Details.MaxHeight;

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Color cInternal = solution.IsCurrentBest && alg == "SA" ? Color.LightGreen : alg == "SA" ? Color.LightBlue : solution.IsCurrentBest ? Color.Orange : Color.Magenta;
                Color cExternal = solution.IsCurrentBest && alg == "SA" ? Color.Green : alg == "SA" ? Color.Blue : solution.IsCurrentBest ? Color.Red : Color.Purple;
                Pen pen = new Pen(cExternal);
                SolidBrush brush = new SolidBrush(cInternal);
                g.Clear(SystemColors.Control);
                if (solution.Details.Points != null)
                {
                    int dR = 4;
                    PointF[] points = solution.Details.Points.Select(x => new PointF((float)(x.Item1 * sX), (float)(x.Item2 * sY))).ToArray();
                    if (solution.IsFinal) g.FillPolygon(brush, points);
                    g.DrawPolygon(pen, points);
                    points.ToList().ForEach(x =>
                    {
                        g.FillEllipse(brush, x.X - dR, x.Y - dR, 2 * dR, 2 * dR);
                        g.DrawEllipse(pen, x.X - dR, x.Y - dR, 2 * dR, 2 * dR);
                    });
                }
                if (solution.Details.Rectangles != null)
                {
                    foreach (var rect in solution.Details.Rectangles)
                    {
                        float x = (float)(rect.Item1 * sX);
                        float y = (float)(bitmap.Height - (rect.Item2 + rect.Item4) * sY) - 1;
                        float w = (float)(rect.Item3 * sX);
                        float h = (float)(rect.Item4 * sY) - 1;
                        g.FillRectangle(brush, x, y, w, h);
                        g.DrawRectangle(pen, x, y, w, h);
                    }
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