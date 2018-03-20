using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace LocalSearchOptimization.Examples.Problems.VehicleRouting
{
    public static class VehicleRoutingSolutionDraw
    {
        public static Bitmap Draw(this VehicleRoutingSolution solution, BitmapStyle bitmapStyle = null)
        {
            BitmapStyle style = bitmapStyle ?? new BitmapStyle();
            double maxSize = Math.Max(solution.MaxWidth, solution.MaxHeight);
            double scaleX = (style.ImageWidth - style.MarginX - 8 * style.Radius) / maxSize;
            double scaleY = (style.ImageHeight - style.MarginY - 4 * style.Radius) / maxSize;
            PointF[][] points = new PointF[solution.NumberOfVehicles][];
            for (int i = 0; i < solution.NumberOfVehicles; i++)
            {
                int startInd = i * solution.VehicleCapacity;
                int endInd = Math.Min((i + 1) * solution.VehicleCapacity, solution.NumberOfCustomers);
                points[i] = new PointF[endInd - startInd];
                for (int j = startInd; j < endInd; j++)
                {
                    int n = solution.Order[j];
                    points[i][j - startInd] = new PointF(style.MarginX + 2 * style.Radius + (float)(solution.X[n] * scaleX), style.ImageHeight - 2 * style.Radius - (float)(solution.Y[n] * scaleY));
                }
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
                for (int i = 0; i < solution.NumberOfVehicles; i++)
                {
                    if (solution.IsFinal)
                        g.FillPolygon(brush, points[i]);
                    if (solution.OperatorTag != "init") g.DrawPolygon(pen, points[i]);
                }
                for (int i = 0; i < solution.NumberOfVehicles; i++)
                    for (int j = 0; j < points[i].Length; j++)
                    {
                        g.FillEllipse(brush, points[i][j].X - style.Radius, points[i][j].Y - style.Radius, 2 * style.Radius, 2 * style.Radius);
                        g.DrawEllipse(pen, points[i][j].X - style.Radius, points[i][j].Y - style.Radius, 2 * style.Radius, 2 * style.Radius);
                        g.DrawString(string.Format("{0}", solution.Order[j] + 1), fontSmall, penBrush, points[i][j].X, points[i][j].Y);
                    }
                g.DrawString(String.Format("Tour lenght: {0:F4} (lower bound gap {1:F2}%){4}\nIterations: {2}\nTime: {3:F3}s", solution.CostValue, solution.LowerBoundGap, solution.IterationNumber, solution.TimeInSeconds, solution.IsCurrentBest ? " <<<" : ""), font, brushBlack, 0, 0);
            }
            return bitmap;
        }
    }
}
