using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples
{
    public static class DrawCostDiagram
    {
        private static List<Color> colors = new List<Color> { Color.Blue, Color.Green, Color.Purple, Color.Lime, Color.Magenta, Color.Maroon, Color.Navy, Color.Orange, Color.Purple, Color.Red, Color.Teal, Color.Yellow, Color.Cyan };

        static public Bitmap Draw(IOptimizationAlgorithm optimizer, BitmapStyle bitmapStyle, int maxPoints = 0)
        {
            BitmapStyle style = bitmapStyle ?? bitmapStyle;
            if ((optimizer?.SearchHistory?.Count ?? 0) == 0) return null;
            int n = maxPoints == 0 ? 1 : optimizer.SearchHistory.Count / maxPoints + 1;
            List<SolutionSummary> historyToDraw = maxPoints == 0 ? optimizer.SearchHistory : new List<SolutionSummary>();
            double minCost = int.MaxValue;
            double maxCost = 0;
            HashSet<string> instances = new HashSet<string>();
            HashSet<string> operators = new HashSet<string>();
            for (int i = 0; i < optimizer.SearchHistory.Count; i++)
            {
                if (minCost > optimizer.SearchHistory[i].CostValue) minCost = optimizer.SearchHistory[i].CostValue;
                if (maxCost < optimizer.SearchHistory[i].CostValue) maxCost = optimizer.SearchHistory[i].CostValue;
                if (maxPoints == 0 || i % n == 0)
                {
                    if (maxPoints > 0) historyToDraw.Add(optimizer.SearchHistory[i]);
                    instances.Add(optimizer.SearchHistory[i].InstanceTag);
                    operators.Add(optimizer.SearchHistory[i].OperatorTag);
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
            double scaleX = (double)(style.ImageWidth - style.MarginX) / (historyToDraw.Count);
            double scaleY = (style.ImageHeight - style.MarginY - 4 * style.CostRadius) / (maxCost - minCost);
            Bitmap bitmap = new Bitmap(style.ImageWidth, style.ImageHeight, PixelFormat.Format32bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Brush brushBlack = new SolidBrush(Color.Black);
                Font font = new Font(style.FontName, style.FontSize);
                g.Clear(Color.FromName(style.BackgroundColor));
                for (int i = 0; i < historyToDraw.Count - 1; i++)
                {
                    float x = style.MarginX + (float)(i * scaleX);
                    float y = bitmap.Height - 4 * style.CostRadius - (float)((historyToDraw[i].CostValue - minCost) * scaleY);
                    g.FillEllipse(instanceBrush.Count > 1 ? instanceBrush[historyToDraw[i].InstanceTag] : operatorBrush[historyToDraw[i].OperatorTag], x, y, 2 * style.CostRadius, 2 * style.CostRadius);
                }
                string summary = String.Format("Max cost: {0:F4}\nMin cost: {1:F4}\nAccepted iterations: {2}\nTime: {3:f3}s", maxCost, minCost, optimizer.SearchHistory.Count, optimizer.CurrentSolution.TimeInSeconds);
                g.DrawString(summary, font, brushBlack, 0, 0);
            }
            return bitmap;
        }
    }
}