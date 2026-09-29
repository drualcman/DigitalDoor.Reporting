using System.Globalization;

namespace DigitalDoor.Reporting.Entities.Helpers.Html;

internal static class CssValue
{
    private const string TransparentColour = "transparent";

    public static string Millimeters(double millimeters)
    {
        return $"{millimeters.ToString("0.####", CultureInfo.InvariantCulture)}mm";
    }

    public static string Points(double points)
    {
        return $"{points.ToString("0.####", CultureInfo.InvariantCulture)}pt";
    }

    public static string Number(double value)
    {
        return value.ToString("0.####", CultureInfo.InvariantCulture);
    }

    public static bool IsTransparent(string colour)
    {
        return string.IsNullOrWhiteSpace(colour) || colour.Trim().Equals(TransparentColour, StringComparison.OrdinalIgnoreCase);
    }

    public static string Colour(string colour)
    {
        return IsTransparent(colour) ? TransparentColour : colour.Trim();
    }
}
