using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples
{
    public static class DrawCostHistory
    {
        static public Bitmap Draw(ISolution solution, BitmapStyle style = null)
        {
            if (solution.SolutionsHistory.Count == 0) return null;
            if (style == null) style = new BitmapStyle();
            List<Color> colors = new List<Color> { Color.Blue, Color.Brown, Color.Cyan, Color.Green, Color.Purple, Color.Lime, Color.Magenta, Color.Maroon, Color.Navy, Color.Olive, Color.Orange, Color.Pink, Color.Purple, Color.Red, Color.Teal, Color.Yellow };
            Bitmap bitmap = new Bitmap(style.BitmapWidth, style.BitmapHeight, PixelFormat.Format32bppRgb);
            double minCost = solution.SolutionsHistory.Min(x => x.Item3);
            double maxCost = solution.SolutionsHistory.Max(x => x.Item3);
            double scaleX = (double)(bitmap.Width - style.MarginX) / (solution.SolutionsHistory.Count);
            double scaleY = (bitmap.Height - style.MarginY - 4 * style.CostRadius) / (maxCost - minCost);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(style.Background);
                for (int i = 0; i < solution.SolutionsHistory.Count - 1; i++)
                {
                    bool parsed = int.TryParse(solution.SolutionsHistory[i].Item1, out int instance);
                    float x = style.MarginX + (float)(i * scaleX);
                    float y = bitmap.Height - 4 * style.CostRadius - (float)((solution.SolutionsHistory[i].Item3 - minCost) * scaleY);
                    g.FillEllipse(parsed ? new SolidBrush(colors[instance % colors.Count]) : style.CostBrush, x, y, 2 * style.CostRadius, 2 * style.CostRadius);
                }
                g.DrawString(String.Format("Max cost: {0}\nMin cost: {1}\nIterations: {1}\nTime: {2}s", Math.Round(maxCost, 4), Math.Round(minCost, 4), solution.SolutionsHistory.Count, Math.Round(solution.TimeInSeconds, 3)), style.Font, new SolidBrush(Color.Black), 0, 0);
            }
            return bitmap;
        }
    }
}