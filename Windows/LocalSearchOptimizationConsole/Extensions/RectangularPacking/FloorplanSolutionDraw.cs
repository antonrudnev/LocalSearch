using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace LocalSearchOptimization.Examples.RectangularPacking
{
    public static class FloorplanSolutionDraw
    {
        public static Bitmap Draw(this FloorplanSolution solution, BitmapStyle bitmapStyle = null)
        {
            lock (DrawCostDiagram.thisLock)
            {
                BitmapStyle style = bitmapStyle ?? new BitmapStyle();
                Bitmap bitmap = new Bitmap(style.ImageWidth, style.ImageHeight, PixelFormat.Format32bppRgb);
                double maxSize = Math.Max(solution.MaxWidth, solution.MaxHeight);
                double scaleX = (bitmap.Width - style.MarginX - 2 * style.Pen.Width) / maxSize;
                double scaleY = (bitmap.Height - style.MarginY - style.Pen.Width) / maxSize;
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(style.Background);
                    if (solution.IsFinal)
                        g.FillRectangle(Brushes.LightGray, style.MarginX + style.Pen.Width, bitmap.Height - style.Pen.Width - (float)(solution.MaxHeight * scaleY), (float)(solution.MaxWidth * scaleX), (float)(solution.MaxHeight * scaleY));
                    for (int i = 0; i < solution.Order.Count; i++)
                    {
                        int n = solution.Order[i];
                        float x = style.MarginX + style.Pen.Width + (float)(solution.X[n] * scaleX);
                        float y = bitmap.Height - style.Pen.Width - (float)((solution.Y[n] + solution.H[n]) * scaleY);
                        float w = (float)(solution.W[n] * scaleX);
                        float h = (float)(solution.H[n] * scaleY);
                        g.FillRectangle(style.Brush, x, y, w, h);
                        g.DrawRectangle(style.Pen, x, y, w, h);
                        g.DrawString(string.Format("{0}", n - 1), style.Font, new SolidBrush(style.Pen.Color), x + w / 2 - style.Font.SizeInPoints, y + h / 2 - style.Font.SizeInPoints);
                    }
                    g.DrawString(String.Format("Packing area: {0} (cost: {1})\nIterations: {2}\nTime: {3}s", Math.Round(solution.MaxWidth * solution.MaxHeight, 4), Math.Round(solution.CostValue, 4), solution.IterationNumber, Math.Round(solution.TimeInSeconds, 3)), style.Font, new SolidBrush(Color.Black), 0, 0);
                }
                return bitmap;
            }
        }
    }
}