namespace DigitalDoor.Reporting.Entities.Layout;

public sealed class LayoutBox
{
    public double Left { get; }
    public double Top { get; }
    public double Width { get; }
    public double Height { get; }
    public double Right => Left + Width;
    public double Bottom => Top + Height;
    public double CenterX => Left + Width / 2;
    public double CenterY => Top + Height / 2;

    public LayoutBox(double left, double top, double width, double height)
    {
        Left = left;
        Top = top;
        Width = width;
        Height = height;
    }

    public LayoutBox Deflate(double left, double top, double right, double bottom)
    {
        return new LayoutBox(Left + left, Top + top, Width - left - right, Height - top - bottom);
    }
}
