﻿using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace LocalSearchOptimization.Examples.Problems.TravellingSalesman
{
    public static class TspSolutionDraw
    {
        public static Bitmap Draw(this TspSolution solution, BitmapStyle bitmapStyle = null)
        {
            BitmapStyle style = bitmapStyle ?? new BitmapStyle();
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
                if (solution.OperatorTag != "init") g.DrawPolygon(pen, points);
                for (int i = 0; i < points.Length; i++)
                {
                    g.FillEllipse(brush, points[i].X - style.Radius, points[i].Y - style.Radius, 2 * style.Radius, 2 * style.Radius);
                    g.DrawEllipse(pen, points[i].X - style.Radius, points[i].Y - style.Radius, 2 * style.Radius, 2 * style.Radius);
                    g.DrawString(string.Format("{0}", solution.Order[i] + 1), fontSmall, penBrush, points[i].X, points[i].Y);
                }
                g.DrawString(String.Format("Tour lenght: {0:F4} (lower bound gap {1:F2}%){4}\nIterations: {2}\nTime: {3:F3}s", solution.CostValue, solution.LowerBoundGap, solution.IterationNumber, solution.TimeInSeconds, solution.IsCurrentBest ? " <<<" : ""), font, brushBlack, 0, 0);
            }
            return bitmap;
        }
    }
}