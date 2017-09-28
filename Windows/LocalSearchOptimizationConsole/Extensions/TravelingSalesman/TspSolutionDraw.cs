using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using LocalSearchOptimization.Components;

namespace LocalSearchOptimization.Examples.Problems.TravelingSalesman
{
    public static class TspSolutionDraw
    {
        public static Bitmap Draw(this TspSolution solution, BitmapStyle style = null)
        {
            if (style == null) style = new BitmapStyle();
            Bitmap bitmap = new Bitmap(style.ImageWidth, style.ImageHeight, PixelFormat.Format32bppRgb);
            double maxSize = Math.Max(solution.MaxWidth, solution.MaxHeight);
            double scaleX = (bitmap.Width - style.MarginX - 4 * style.CityRadius) / maxSize;
            double scaleY = (bitmap.Height - style.MarginY - 4 * style.CityRadius) / maxSize;
            PointF[] points = new PointF[solution.NumberOfCities];
            for (int i = 0; i < solution.NumberOfCities; i++)
            {
                int n = solution.Order[i];
                points[i] = new PointF(style.MarginX + 2 * style.CityRadius + (float)(solution.X[n] * scaleX), bitmap.Height - 2 * style.CityRadius - (float)(solution.Y[n] * scaleY));
            }
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(style.Background);
                if (solution.IsFinal) g.FillPolygon(style.Brush, points);
                g.DrawPolygon(style.Pen, points);
                points.ToList().ForEach(x =>
                {
                    g.FillEllipse(style.Brush, x.X - style.CityRadius, x.Y - style.CityRadius, 2 * style.CityRadius, 2 * style.CityRadius);
                    g.DrawEllipse(style.Pen, x.X - style.CityRadius, x.Y - style.CityRadius, 2 * style.CityRadius, 2 * style.CityRadius);
                });
                g.DrawString(String.Format("Tour lenght: {0}\nIterations: {1}\nTime: {2}s", Math.Round(solution.CostValue, 4), solution.IterationNumber, Math.Round(solution.TimeInSeconds, 3)), style.Font, new SolidBrush(Color.Black), 0, 0);
            }
            return bitmap;
        }

        public static Bitmap DrawCost(this ISolution solution, BitmapStyle style = null)
        {
            return DrawCostHistory.Draw(solution, style);
        }
    }
}