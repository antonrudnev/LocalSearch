using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace LocalSearchOptimization.Examples.RectangularPacking
{
    public static class FloorplanSolutionDraw
    {
        public static Bitmap Draw(this FloorplanSolution solution, BitmapStyle bitmapStyle = null)
        {
            BitmapStyle style = bitmapStyle ?? new BitmapStyle();

            double maxWidth = solution.Transcoder % 2 == 0 ? solution.MaxWidth : solution.MaxHeight;
            double maxHeight = solution.Transcoder % 2 == 0 ? solution.MaxHeight : solution.MaxWidth;
            double[] X = solution.Transcoder % 2 == 0 ? solution.X : solution.Y;
            double[] Y = solution.Transcoder % 2 == 0 ? solution.Y : solution.X;
            double[] W = solution.Transcoder % 2 == 0 ? solution.W : solution.H;
            double[] H = solution.Transcoder % 2 == 0 ? solution.H : solution.W;
            double maxSize = Math.Max(maxWidth, maxHeight);
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
                    g.FillRectangle(Brushes.LightGray, style.MarginX + style.PenWidth, bitmap.Height - style.PenWidth - (float)(maxHeight * scaleY), (float)(maxWidth * scaleX), (float)(maxHeight * scaleY));
                for (int i = 0; i < solution.Order.Count; i++)
                {
                    int n = solution.Order[i];
                    float x = style.MarginX + style.PenWidth + (float)((solution.Transcoder == 1 || solution.Transcoder == 2 ? maxWidth - X[n] - W[n] : X[n]) * scaleX);
                    float y = bitmap.Height - style.PenWidth - (float)((solution.Transcoder == 2 || solution.Transcoder == 3 ? maxHeight - Y[n] : Y[n] + H[n]) * scaleY);
                    float w = (float)(W[n] * scaleX);
                    float h = (float)(H[n] * scaleY);
                    g.FillRectangle(brush, x, y, w, h);
                    g.DrawRectangle(pen, x, y, w, h);
                    g.DrawString(string.Format("{0}", n), fontSmall, penBrush, x + w / 2 - style.FontSize, y + h / 2 - style.FontSize);
                }
                string summary = solution.CostValue == maxWidth * maxHeight
                    ? String.Format("Packing area: {0:F4}{3}\nIterations: {1}\nTime: {2:F3}s", solution.CostValue, solution.IterationNumber, solution.TimeInSeconds, solution.IsCurrentBest ? " <<<" : "")
                    : String.Format("Packing cost: {0:F4} (area: {1:F4}){4}\nIterations: {2}\nTime: {3:F3}s", solution.CostValue, solution.MaxWidth * solution.MaxHeight, solution.IterationNumber, solution.TimeInSeconds, solution.IsCurrentBest ? " <<<" : "");
                g.DrawString(summary, font, brushBlack, 0, 0);
            }
            return bitmap;
        }
    }
}