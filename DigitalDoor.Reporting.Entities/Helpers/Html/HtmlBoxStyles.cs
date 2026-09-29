using DigitalDoor.Reporting.Entities.Layout;

namespace DigitalDoor.Reporting.Entities.Helpers.Html;

internal static class HtmlBoxStyles
{
    public static string Box(LayoutBox box)
    {
        return $"position:absolute;box-sizing:border-box;margin:0;left:{CssValue.Millimeters(box.Left)};top:{CssValue.Millimeters(box.Top)};" +
            $"width:{CssValue.Millimeters(Math.Max(0, box.Width))};height:{CssValue.Millimeters(Math.Max(0, box.Height))};";
    }

    public static string Background(string background)
    {
        return CssValue.IsTransparent(background) ? string.Empty : $"background-color:{CssValue.Colour(background)};";
    }

    public static string Borders(Border borders)
    {
        StringBuilder style = new StringBuilder();
        if (borders is not null)
        {
            string borderStyle = ReportBorderLayout.GetEffectiveStyle(borders).ToString();
            style.Append(Side("top", borders, borders.Top, borderStyle))
                .Append(Side("right", borders, borders.Right, borderStyle))
                .Append(Side("bottom", borders, borders.Bottom, borderStyle))
                .Append(Side("left", borders, borders.Left, borderStyle));
            double[] radii = ReportBorderLayout.GetCornerRadii(borders);
            if (radii.Any(radius => radius > 0))
            {
                style.Append($"border-radius:{string.Join(" ", radii.Select(CssValue.Millimeters))};");
            }
        }
        return style.ToString();
    }

    public static string RotationAndOpacity(Format format)
    {
        StringBuilder style = new StringBuilder();
        if (format.Angle != 0)
        {
            style.Append($"transform:rotate({CssValue.Number(format.Angle)}deg);transform-origin:center center;");
        }
        float opacity = ReportCellLayout.GetOpacity(format);
        if (opacity < 1f)
        {
            style.Append($"opacity:{CssValue.Number(opacity)};");
        }
        return style.ToString();
    }

    private static string Side(string sideName, Border borders, Shade side, string borderStyle)
    {
        double width = ReportBorderLayout.GetVisibleWidth(borders, side);
        return width > 0 ?
            $"border-{sideName}:{CssValue.Millimeters(width)} {borderStyle} {CssValue.Colour(side.Colour)};" :
            $"border-{sideName}:none;";
    }
}
