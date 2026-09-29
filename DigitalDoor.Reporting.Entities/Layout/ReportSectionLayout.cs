namespace DigitalDoor.Reporting.Entities.Layout;

public sealed class ReportSectionLayout
{
    public LayoutBox Bounds { get; }
    public double ContentLeft { get; }
    public double ContentTop { get; }
    public double NextSectionTop { get; }

    private ReportSectionLayout(LayoutBox bounds, double contentLeft, double contentTop, double nextSectionTop)
    {
        Bounds = bounds;
        ContentLeft = contentLeft;
        ContentTop = contentTop;
        NextSectionTop = nextSectionTop;
    }

    public static ReportSectionLayout Create(Format sectionFormat, double left, double top)
    {
        Kernel margin = sectionFormat?.Margin ?? new Kernel();
        Kernel position = sectionFormat?.Position ?? new Kernel();
        Kernel padding = sectionFormat?.Padding ?? new Kernel();
        Dimension dimension = sectionFormat?.Dimension ?? new Dimension();
        double boxLeft = left + (double)(margin.Left + position.Left);
        double boxTop = top + (double)(margin.Top + position.Top);
        LayoutBox bounds = new LayoutBox(boxLeft, boxTop, dimension.Width, dimension.Height);
        double nextSectionTop = top + (double)(margin.Top + margin.Bottom) + dimension.Height;
        return new ReportSectionLayout(bounds, boxLeft + (double)padding.Left, boxTop + (double)padding.Top, nextSectionTop);
    }
}
