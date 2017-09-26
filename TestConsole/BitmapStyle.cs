using System.Drawing;

namespace TestConsole
{
    public class BitmapStyle
    {
        public int BitmapWidth { get; set; } = 3200;
        public int BitmapHeight { get; set; } = 1800;
        public Font Font { get; set; } = new Font("Arial", 24, FontStyle.Bold);
        public Pen Pen { get; set; } = new Pen(Color.Green, 4);
        public Brush Brush { get; set; } = new SolidBrush(Color.LightGreen);
        public Brush CostBrush { get; set; } = new SolidBrush(Color.Blue);
        public Color Background { get; set; } = Color.White;
        public int CostRadius { get; set; } = 5;
        public int CityRadius { get; set; } = 15;
    }
}