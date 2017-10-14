package localsearchoptimization.components;

import java.awt.*;

public class ImageStyle {
    public int imageWidth = 3200;
    public int imageHeight = 1800;
    public int marginX = 0;
    public int marginY = 150;
    public String fontName = "Microsoft Sans Serif";
    public int fontSize = 36;
    public Color penColor = Color.decode("#006400");
    public int penWidth = 5;
    public Color fillColor = Color.decode("#90EE90");
    public int radius = 15;
    public Color backgroundColor = Color.WHITE;

    public ImageStyle() {

    }

    public ImageStyle(ImageStyle style) {
        imageWidth = style.imageWidth;
        imageHeight = style.imageHeight;
        marginX = style.marginX;
        marginY = style.marginY;
        fontName = style.fontName;
        fontSize = style.fontSize;
        penColor = style.penColor;
        penWidth = style.penWidth;
        fillColor = style.fillColor;
        radius = style.radius;
        backgroundColor = style.backgroundColor;
    }
}