using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace LocalSearchOptimization.Examples.Problems.TravelingSalesman
{
    public static class TspSolutionDraw
    {
        public static Bitmap Draw(this TspSolution solution, BitmapStyle bitmapStyle = null)
        {
            BitmapStyle style = bitmapStyle ?? bitmapStyle;
            double maxSize = Math.Max(solution.MaxWidth, solution.MaxHeight);
            double scaleX = (style.ImageWidth - style.MarginX - 8 * style.Radius) / maxSize;
            double scaleY = (style.ImageHeight - style.MarginY - 4 * style.Radius) / maxSize;
            PointF[] points = new PointF[solution.NumberOfCities];
            for (int i = 0; i < solution.NumberOfCities; i++)
            {
                int n = solution.Order[i];
                points[i] = new PointF(style.MarginX + 2 * style.Radius + (float)(solution.X[n] * scaleX), style.ImageHeight - 2 * style.Radius - (float)(solution.Y[n] * scaleY));
            }
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
                    g.FillPolygon(brush, points);
                g.DrawPolygon(pen, points);
                for (int i = 0; i < points.Length; i++)
                {
                    g.FillEllipse(brush, points[i].X - style.Radius, points[i].Y - style.Radius, 2 * style.Radius, 2 * style.Radius);
                    g.DrawEllipse(pen, points[i].X - style.Radius, points[i].Y - style.Radius, 2 * style.Radius, 2 * style.Radius);
                    g.DrawString(string.Format("{0}", solution.Order[i] + 1), fontSmall, penBrush, points[i].X, points[i].Y);
                }
                g.DrawString(String.Format("Tour lenght: {0}\nIterations: {1}\nTime: {2}s", Math.Round(solution.CostValue, 4), solution.IterationNumber, Math.Round(solution.TimeInSeconds, 3)), font, brushBlack, 0, 0);
            }
            return bitmap;
        }
    }
}