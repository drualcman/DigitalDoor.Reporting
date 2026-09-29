namespace DigitalDoor.Reporting.Entities.Layout;

public static class ReportCellLayout
{
    public const double LineHeightFactor = 1.15;

    public static LayoutBox GetCellBox(Format format, double originLeft, double originTop)
    {
        Kernel position = format.Position ?? new Kernel();
        Kernel margin = format.Margin ?? new Kernel();
        Dimension dimension = format.Dimension ?? new Dimension();
        return new LayoutBox(
            originLeft + (double)(position.Left + margin.Left),
            originTop + (double)(position.Top + margin.Top),
            dimension.Width,
            dimension.Height);
    }

    public static LayoutBox GetPaddingBox(LayoutBox cellBox, Format format)
    {
        Border borders = format.Borders ?? new Border();
        return cellBox.Deflate(
            ReportBorderLayout.GetVisibleWidth(borders, borders.Left),
            ReportBorderLayout.GetVisibleWidth(borders, borders.Top),
            ReportBorderLayout.GetVisibleWidth(borders, borders.Right),
            ReportBorderLayout.GetVisibleWidth(borders, borders.Bottom));
    }

    public static LayoutBox GetContentBox(LayoutBox cellBox, Format format)
    {
        Kernel padding = format.Padding ?? new Kernel();
        return GetPaddingBox(cellBox, format).Deflate(
            (double)padding.Left, (double)padding.Top, (double)padding.Right, (double)padding.Bottom);
    }

    public static float GetOpacity(Format format)
    {
        float opacity = format.FontDetails?.ColorSize?.Opacity ?? 1f;
        return Math.Clamp(opacity, 0f, 1f);
    }
}
