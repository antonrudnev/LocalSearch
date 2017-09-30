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
            BitmapStyle style = bitmapStyle ?? new BitmapStyle();
            double maxSize = Math.Max(solution.MaxWidth, solution.MaxHeight);
            double scaleX = (style.ImageWidth - style.MarginX - 8 * style.CityRadius) / maxSize;
            double scaleY = (style.ImageHeight - style.MarginY - 4 * style.CityRadius) / maxSize;
            PointF[] points = new PointF[solution.NumberOfCities];
            for (int i = 0; i < solution.NumberOfCities; i++)
            {
                int n = solution.Order[i];
                points[i] = new PointF(style.MarginX + 2 * style.CityRadius + (float)(solution.X[n] * scaleX), style.ImageHeight - 2 * style.CityRadius - (float)(solution.Y[n] * scaleY));
            }
            lock (DrawCostDiagram.thisLock)
            {
                Bitmap bitmap = new Bitmap(style.ImageWidth, style.ImageHeight, PixelFormat.Format32bppRgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(style.Background);
                    if (solution.IsFinal)
                        g.FillPolygon(style.Brush, points);
                    g.DrawPolygon(style.Pen, points);
                    for (int i = 0; i < points.Length; i++)
                    {
                        g.FillEllipse(style.Brush, points[i].X - style.CityRadius, points[i].Y - style.CityRadius, 2 * style.CityRadius, 2 * style.CityRadius);
                        g.DrawEllipse(style.Pen, points[i].X - style.CityRadius, points[i].Y - style.CityRadius, 2 * style.CityRadius, 2 * style.CityRadius);
                        g.DrawString(string.Format("{0}", solution.Order[i] + 1), new Font(style.Font.Name, style.Font.Size - 2), new SolidBrush(style.Pen.Color), points[i].X, points[i].Y);
                    }
                    g.DrawString(String.Format("Tour lenght: {0}\nIterations: {1}\nTime: {2}s", Math.Round(solution.CostValue, 4), solution.IterationNumber, Math.Round(solution.TimeInSeconds, 3)), style.Font, new SolidBrush(Color.Black), 0, 0);
                }
                return bitmap;
            }
        }
    }
}