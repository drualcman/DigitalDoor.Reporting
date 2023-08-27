namespace DigitalDoor.Reporting.Entities.ValueObjects;

public class Font
{
    public string FontName { get { return FontNameBK; } set { FontNameBK = value; } }
    private string FontNameBK;
    public Shade ColorSize { get { return ColorSizeBK; } set { ColorSizeBK = value; } }
    private Shade ColorSizeBK;
    public FontStyle FontStyle { get { return FontStyleBK; } set { FontStyleBK = value; } }
    private FontStyle FontStyleBK;
    public Font()
    {
        FontNameBK = "Arial";
        ColorSizeBK = new Shade(10);
        FontStyleBK = new FontStyle();
    }
    public Font(string fontName, Shade colorSize, FontStyle fontStyle) : this() =>
        (FontNameBK, ColorSizeBK, FontStyleBK) = (fontName, colorSize, fontStyle);
    public Font(Shade colorSize) : this() => ColorSizeBK = colorSize;
    public Font(FontStyle fontStyle) : this() => FontStyleBK = fontStyle;
    public Font(Shade colorSize, FontStyle fontStyle) : this(colorSize) { FontStyleBK = fontStyle; }
    public Font(string fontName) : this() => FontNameBK = fontName;
    public Font(int fontWeight) : this(new FontStyle(fontWeight)) { }
    public Font(int size, string colorName) : this(new Shade(size, colorName)) { }
    public Font(string fontName, Shade colorSize) : this(colorSize) { FontNameBK = fontName; }
    public Font(string fontName, int size) : this(fontName, new Shade(size)) { }
    public Font(string fontName, int size, string colorName) : this(fontName, new Shade(size, colorName)) { }

}
