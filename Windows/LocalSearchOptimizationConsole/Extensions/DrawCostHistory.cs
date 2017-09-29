using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples
{
    public static class DrawCostHistory
    {
        private static List<Color> colors = new List<Color> { Color.Blue, Color.Green, Color.Purple, Color.Lime, Color.Magenta, Color.Maroon, Color.Navy, Color.Orange, Color.Purple, Color.Red, Color.Teal, Color.Yellow, Color.Cyan };

        static public Bitmap Draw(ISolution solution, BitmapStyle bitmapStyle)
        {
            BitmapStyle style = bitmapStyle ?? new BitmapStyle();
            if (solution.SolutionsHistory.Count == 0) return null;
            int n = solution.SolutionsHistory.Count / 100000 + 1;
            List<SolutionSummary> solutionsHistory = new List<SolutionSummary>();
            double minCost = int.MaxValue;
            double maxCost = 0;
            HashSet<string> instances = new HashSet<string>();
            HashSet<string> operators = new HashSet<string>();
            for (int i = 0; i < solution.SolutionsHistory.Count; i++)
            {
                if (minCost > solution.SolutionsHistory[i].CostValue) minCost = solution.SolutionsHistory[i].CostValue;
                if (maxCost < solution.SolutionsHistory[i].CostValue) maxCost = solution.SolutionsHistory[i].CostValue;
                if (i % n == 0)
                {
                    SolutionSummary solutionSummary = solution.SolutionsHistory[i];
                    solutionsHistory.Add(solutionSummary);
                    instances.Add(solutionSummary.InstanceTag);
                    operators.Add(solutionSummary.OperatorTag);
                }
            }
            Dictionary<string, Brush> instanceBrush = new Dictionary<string, Brush>();
            int counter = 0;
            foreach (string instance in instances)
            {
                instanceBrush.Add(instance, new SolidBrush(colors[counter % colors.Count]));
                counter++;
            }
            Dictionary<string, Brush> operatorBrush = new Dictionary<string, Brush>();
            counter = 0;
            foreach (string operation in operators)
            {
                operatorBrush.Add(operation, new SolidBrush(colors[counter % colors.Count]));
                if (operation != "init" && operation != "shuffle") counter++;
            }
            Bitmap bitmap = new Bitmap(style.ImageWidth, style.ImageHeight, PixelFormat.Format32bppRgb);
            double scaleX = (double)(bitmap.Width - style.MarginX) / (solutionsHistory.Count);
            double scaleY = (bitmap.Height - style.MarginY - 4 * style.CostRadius) / (maxCost - minCost);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(style.Background);
                for (int i = 0; i < solutionsHistory.Count - 1; i++)
                {
                    float x = style.MarginX + (float)(i * scaleX);
                    float y = bitmap.Height - 4 * style.CostRadius - (float)((solutionsHistory[i].CostValue - minCost) * scaleY);
                    g.FillEllipse(instanceBrush.Count > 1 ? instanceBrush[solutionsHistory[i].InstanceTag] : operatorBrush[solutionsHistory[i].OperatorTag], x, y, 2 * style.CostRadius, 2 * style.CostRadius);
                }
                string summary = String.Format("Max cost: {0:F4}\nMin cost: {1:F4}\nAccepted iterations: {2}\nTime: {3:f3}s", maxCost, minCost, solution.SolutionsHistory.Count, solution.TimeInSeconds);
                g.DrawString(summary, style.Font, new SolidBrush(Color.Black), 0, 0);
            }
            return bitmap;
        }
    }
}