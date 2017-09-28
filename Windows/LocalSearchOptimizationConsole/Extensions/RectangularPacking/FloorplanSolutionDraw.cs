using System;
using System.Drawing;
using System.Drawing.Imaging;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.RectangularPacking
{
    public static class FloorplanSolutionDraw
    {
        public static Bitmap Draw(this FloorplanSolution solution, BitmapStyle style = null)
        {
            if (style == null) style = new BitmapStyle();
            Bitmap bitmap = new Bitmap(style.ImageWidth, style.ImageHeight, PixelFormat.Format32bppRgb);
            double maxSize = Math.Max(solution.MaxWidth, solution.MaxHeight);
            double scaleX = (bitmap.Width - style.MarginX) / maxSize;
            double scaleY = (bitmap.Height - style.MarginY) / maxSize;
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(style.Background);
                for (int i = 0; i < solution.Order.Count; i++)
                {
                    int n = solution.Order[i];
                    float x = style.MarginX + (float)(solution.X[n] * scaleX);
                    float y = bitmap.Height - (float)((solution.Y[n] + solution.H[n]) * scaleY);
                    float w = (float)(solution.W[n] * scaleX);
                    float h = (float)(solution.H[n] * scaleY);
                    g.FillRectangle(style.Brush, x, y, w, h);
                    g.DrawRectangle(style.Pen, x, y, w, h);
                    g.DrawString(string.Format("{0}", n - 1), style.Font, new SolidBrush(Color.Black), x, y);
                }
                g.DrawString(String.Format("Packing area: {0}\nCost value: {1}\nIterations: {2}\nTime: {3}s", Math.Round(solution.MaxWidth * solution.MaxHeight, 4), Math.Round(solution.CostValue, 4), solution.IterationNumber, Math.Round(solution.TimeInSeconds, 3)), style.Font, new SolidBrush(Color.Black), 0, 0);
            }
            return bitmap;
        }

        public static Bitmap DrawCost(this ISolution solution, BitmapStyle style = null)
        {
            return DrawCostHistory.Draw(solution, style);
        }
    }
}