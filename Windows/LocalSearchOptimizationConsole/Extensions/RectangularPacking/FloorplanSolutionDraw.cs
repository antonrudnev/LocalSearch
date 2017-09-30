using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace LocalSearchOptimization.Examples.RectangularPacking
{
    public static class FloorplanSolutionDraw
    {
        public static Bitmap Draw(this FloorplanSolution solution, BitmapStyle bitmapStyle = null)
        {
            BitmapStyle style = bitmapStyle ?? bitmapStyle;
            double maxSize = Math.Max(solution.MaxWidth, solution.MaxHeight);
            double scaleX = (style.ImageWidth - style.MarginX - 2 * style.PenWidth) / maxSize;
            double scaleY = (style.ImageHeight - style.MarginY - style.PenWidth) / maxSize;
            Bitmap bitmap = new Bitmap(style.ImageWidth, style.ImageHeight, PixelFormat.Format32bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Pen pen = new Pen(Color.FromName(style.PenColor), style.PenWidth);
                Brush brush = new SolidBrush(Color.FromName(style.BrushColor));
                Brush penBrush = new SolidBrush(Color.FromName(style.PenColor));
                Brush brushBlack = new SolidBrush(Color.Black);
                Font font = new Font(style.FontName, style.FontSize);
                Font fontSmall = new Font(style.FontName, style.FontSize - 2);
                g.Clear(Color.FromName(style.BackgroundColor));
                if (solution.IsFinal)
                    g.FillRectangle(Brushes.LightGray, style.MarginX + style.PenWidth, bitmap.Height - style.PenWidth - (float)(solution.MaxHeight * scaleY), (float)(solution.MaxWidth * scaleX), (float)(solution.MaxHeight * scaleY));
                for (int i = 0; i < solution.Order.Count; i++)
                {
                    int n = solution.Order[i];
                    float x = style.MarginX + style.PenWidth + (float)(solution.X[n] * scaleX);
                    float y = bitmap.Height - style.PenWidth - (float)((solution.Y[n] + solution.H[n]) * scaleY);
                    float w = (float)(solution.W[n] * scaleX);
                    float h = (float)(solution.H[n] * scaleY);
                    g.FillRectangle(brush, x, y, w, h);
                    g.DrawRectangle(pen, x, y, w, h);
                    g.DrawString(string.Format("{0}", n), fontSmall, penBrush, x + w / 2 - style.FontSize, y + h / 2 - style.FontSize);
                }
                g.DrawString(String.Format("Packing area: {0} (cost: {1})\nIterations: {2}\nTime: {3}s", Math.Round(solution.MaxWidth * solution.MaxHeight, 4), Math.Round(solution.CostValue, 4), solution.IterationNumber, Math.Round(solution.TimeInSeconds, 3)), font, brushBlack, 0, 0);
            }
            return bitmap;
        }
    }
}