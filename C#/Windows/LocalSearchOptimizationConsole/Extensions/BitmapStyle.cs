namespace LocalSearchOptimization.Examples
{
    public class BitmapStyle
    {
        public int ImageWidth { get; set; } = 3200;
        public int ImageHeight { get; set; } = 1800;
        public int MarginX { get; set; } = 0;
        public int MarginY { get; set; } = 150;
        public string FontName { get; set; } = "Microsoft Sans Serif";
        public int FontSize { get; set; } = 24;
        public string PenColor { get; set; } = "Green";
        public int PenWidth { get; set; } = 5;
        public string BrushColor { get; set; } = "LightGreen";
        public int Radius { get; set; } = 15;
        public string BackgroundColor { get; set; } = "White";

        public BitmapStyle()
        {

        }

        public BitmapStyle(BitmapStyle style)
        {
            ImageWidth = style.ImageWidth;
            ImageHeight = style.ImageHeight;
            MarginX = style.MarginX;
            MarginY = style.MarginY;
            FontName = style.FontName;
            FontSize = style.FontSize;
            PenColor = style.PenColor;
            PenWidth = style.PenWidth;
            BrushColor = style.BrushColor;
            Radius = style.Radius;
            BackgroundColor = style.BackgroundColor;
        }
    }
}