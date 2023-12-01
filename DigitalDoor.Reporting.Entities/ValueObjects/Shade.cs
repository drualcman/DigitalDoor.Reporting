namespace DigitalDoor.Reporting.Entities.ValueObjects;

public class Shade
{
    public double Width { get { return WidthBK; } set { WidthBK = value; } }
    private double WidthBK;
    public string Colour { get { return ColourBK; } set { ColourBK = value; } }
    private string ColourBK;

    public float Opacity { get { return OpacityBK; } set { OpacityBK = value; } }
    private float OpacityBK;

    public Shade()
    {
        Opacity = 1;
        WidthBK = 0;
        ColourBK = "black";
    }
    public Shade(double with) : this() => WidthBK = with;
    public Shade(double with, string color) : this() => (WidthBK, ColourBK) = (with, color);

    public Shade(double with, string color, float opacity) : this() => (WidthBK, ColourBK,OpacityBK) = (with, color,opacity);



}
