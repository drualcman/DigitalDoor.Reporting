using DigitalDoor.Reporting.Entities.Layout;

namespace DigitalDoor.Reporting.Entities.Helpers.Html;

internal static class HtmlTextStyles
{
    private const int BoldWeightThreshold = 600;
    private const string SansSerifFamilies = "Arial, 'Liberation Sans', Arimo, Helvetica, sans-serif";
    private const string SerifFamilies = "'Times New Roman', 'Liberation Serif', Tinos, Times, serif";
    private const string MonospaceFamilies = "'Courier New', 'Liberation Mono', Cousine, Courier, monospace";

    public static string Text(Format format)
    {
        Font fontDetails = format.FontDetails ?? new Font();
        FontStyle fontStyle = fontDetails.FontStyle ?? new FontStyle();
        Kernel padding = format.Padding ?? new Kernel();
        return $"font-family:{FontFamilies(fontDetails.FontName)};" +
            $"font-size:{CssValue.Points(Math.Max(0, fontDetails.ColorSize?.Width ?? 0))};" +
            $"font-weight:{(fontStyle.Bold >= BoldWeightThreshold ? "700" : "400")};" +
            $"font-style:{(fontStyle.Italic ? "italic" : "normal")};" +
            $"color:{CssValue.Colour(fontDetails.ColorSize?.Colour ?? "black")};" +
            $"text-align:{Alignment(format.TextAlignment)};" +
            $"text-decoration:{Decoration(format.TextDecoration)};" +
            $"line-height:{CssValue.Number(ReportCellLayout.LineHeightFactor)};" +
            "white-space:pre-line;overflow-wrap:anywhere;letter-spacing:0;text-transform:none;" +
            $"padding:{CssValue.Millimeters((double)padding.Top)} {CssValue.Millimeters((double)padding.Right)} " +
            $"{CssValue.Millimeters((double)padding.Bottom)} {CssValue.Millimeters((double)padding.Left)};";
    }

    private static string FontFamilies(string fontName)
    {
        string name = string.IsNullOrWhiteSpace(fontName) ? "Arial" : fontName.Trim();
        string baseName = name.Split('-')[0].Trim();
        string families = baseName.ToLowerInvariant() switch
        {
            "arial" => SansSerifFamilies,
            "helvetica" => SansSerifFamilies,
            "times" => SerifFamilies,
            "times new roman" => SerifFamilies,
            "courier" => MonospaceFamilies,
            "courier new" => MonospaceFamilies,
            _ => $"'{name.Replace("'", "")}', {SansSerifFamilies}"
        };
        return families;
    }

    private static string Alignment(TextAlignment alignment)
    {
        string cssAlignment = alignment switch
        {
            TextAlignment.Right => "right",
            TextAlignment.Center => "center",
            TextAlignment.Justify => "justify",
            _ => "left"
        };
        return cssAlignment;
    }

    private static string Decoration(TextDecoration decoration)
    {
        string cssDecoration = decoration switch
        {
            TextDecoration.Underline => "underline",
            TextDecoration.Line => "line-through",
            TextDecoration.Overline => "overline",
            _ => "none"
        };
        return cssDecoration;
    }
}
