using System.Drawing;

namespace LocalSearchOptimization.Examples
{
    public class BitmapStyle
    {
        public int ImageWidth { get; set; } = 3200;
        public int ImageHeight { get; set; } = 1800;
        public int MarginX { get; set; } = 0;
        public int MarginY { get; set; } = 150;
        public Font Font { get; set; } = new Font("Arial", 24);
        public Pen Pen { get; set; } = new Pen(Color.Green, 5);
        public Brush Brush { get; set; } = Brushes.LightGreen;
        public Brush CostBrush { get; set; } = Brushes.Blue;
        public Color Background { get; set; } = Color.White;
        public int CostRadius { get; set; } = 5;
        public int CityRadius { get; set; } = 15;

        public BitmapStyle Clone()
        {
            return new BitmapStyle
            {
                ImageWidth = ImageWidth,
                ImageHeight = ImageHeight,
                MarginX = MarginX,
                MarginY = MarginY,
                Font = Font,
                Pen = new Pen(Pen.Color, Pen.Width),
                Brush = Brush,
                CostBrush = CostBrush,
                Background = Background,
                CostRadius = CostRadius,
                CityRadius = CityRadius
            };
        }
    }
}