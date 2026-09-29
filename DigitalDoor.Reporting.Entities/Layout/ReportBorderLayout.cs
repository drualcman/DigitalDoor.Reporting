namespace DigitalDoor.Reporting.Entities.Layout;

public static class ReportBorderLayout
{
    public static BorderStyle GetEffectiveStyle(Border borders)
    {
        BorderStyle style = borders?.Style ?? BorderStyle.hidden;
        return style == BorderStyle.none ? BorderStyle.solid : style;
    }

    public static double GetVisibleWidth(Border borders, Shade side)
    {
        bool isVisible = borders is not null && side is not null && side.Width > 0 && GetEffectiveStyle(borders) != BorderStyle.hidden;
        return isVisible ? side.Width : 0;
    }

    public static bool HasSameBorderOnAllSides(Border borders)
    {
        Shade[] sides = GetSides(borders);
        return sides.All(side => side is not null) &&
            sides.All(side => GetVisibleWidth(borders, side) == GetVisibleWidth(borders, borders.Top)) &&
            sides.All(side => string.Equals(side.Colour?.Trim(), borders.Top.Colour?.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static bool UsesCornerRadius(Border borders)
    {
        bool allSidesHidden = borders is not null && GetSides(borders).All(side => GetVisibleWidth(borders, side) == 0);
        return borders is not null && (allSidesHidden || HasSameBorderOnAllSides(borders));
    }

    public static double[] GetCornerRadii(Border borders)
    {
        double[] radii = { 0, 0, 0, 0 };
        if (UsesCornerRadius(borders))
        {
            radii[0] = (double)Math.Max(0, borders.Top?.Radius?.Left ?? 0);
            radii[1] = (double)Math.Max(0, borders.Top?.Radius?.Right ?? 0);
            radii[2] = (double)Math.Max(0, borders.Bottom?.Radius?.Right ?? 0);
            radii[3] = (double)Math.Max(0, borders.Bottom?.Radius?.Left ?? 0);
        }
        return radii;
    }

    private static Shade[] GetSides(Border borders)
    {
        return new[] { borders.Top, borders.Right, borders.Bottom, borders.Left };
    }
}
