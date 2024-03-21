namespace DigitalDoor.Reporting.Entities.ValueObjects;

public class Shade
{
    public double Width { get { return WidthBK; } set { WidthBK = value; } }
    private double WidthBK;
    public string Colour { get { return ColourBK; } set { ColourBK = value; } }
    private string ColourBK;

    public float Opacity { get { return OpacityBK; } set { OpacityBK = value; } }
    private float OpacityBK;
    public Kernel Radius { get { return RadiusBK; } set { RadiusBK = value; } }
    private Kernel RadiusBK;

    public Shade()
    {
        Opacity = 1;
        WidthBK = 0;
        ColourBK = "black";
        Radius = new Kernel();
    }
    public Shade(double with) : this() => WidthBK = with;
    public Shade(double with, string color) : this(with) => ColourBK = color;

    public Shade(double with, string color, float opacity) : this(with, color) => OpacityBK = opacity;
    public Shade(Kernel radius) : this() => RadiusBK = radius;
    public Shade(double with, Kernel radius) : this(radius) => WidthBK = with;
    public Shade(double with, string color, Kernel radius) : this(with, radius) => ColourBK = color;
    public Shade(double with, string color, Kernel radius, float opacity) : this(with, color, radius) => OpacityBK = opacity;

}
