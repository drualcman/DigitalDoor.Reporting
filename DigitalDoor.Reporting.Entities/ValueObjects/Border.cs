namespace DigitalDoor.Reporting.Entities.ValueObjects;

public class Border
{
    public Shade Right { get { return RightBK; } set { RightBK = value; } }
    private Shade RightBK;
    public Shade Top { get { return TopBK; } set { TopBK = value; } }
    private Shade TopBK;

    public Shade Bottom { get { return BottomBK; } set { BottomBK = value; } }
    private Shade BottomBK;
    public Shade Left { get { return LeftBK; } set { LeftBK = value; } }
    private Shade LeftBK;
    public BorderStyle Style { get { return StyleBK; } set { StyleBK = value; } }
    private BorderStyle StyleBK; 
    public Kernel Radius { get { return RadiusBK; } set { RadiusBK = value; } }
    private Kernel RadiusBK;

    public Border()
    {
        RightBK = new Shade();
        TopBK = new Shade();
        BottomBK = new Shade();
        LeftBK = new Shade();
        StyleBK = BorderStyle.none;
        Radius = new Kernel();
    }

    public Border(Kernel radius) : this() { Radius = radius; }
    public Border(BorderStyle style, Kernel radius) : this(new Shade(1), style, radius) { }
    public Border(BorderStyle style) : this(new Shade(1), style) { }
    public Border(Shade top, Shade right, Shade bottom, Shade left, BorderStyle style) : this() =>
        (TopBK, RightBK, BottomBK, LeftBK, StyleBK) = (top, right, bottom, left, style);
    public Border(Shade top, Shade right, Shade bottom, Shade left) : this(top, right, bottom, left, BorderStyle.none) { }
    public Border(Shade all) : this(all, all, all, all) { }
    public Border(Shade all, BorderStyle style) : this(all, all, all, all, style) { }
    public Border(Shade all, Kernel radius) : this(all) { Radius = radius; }
    public Border(Shade all, BorderStyle style, Kernel radius) : this(all, style) { Radius = radius; }

}
