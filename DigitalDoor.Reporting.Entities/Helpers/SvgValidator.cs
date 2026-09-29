namespace DigitalDoor.Reporting.Entities.Helpers;

public static class SvgValidator
{
    private const string SvgOpeningTag = "<svg";

    public static bool IsSvg(string value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
            value.IndexOf(SvgOpeningTag, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool TryGetSvg(object value, out string svg)
    {
        string candidate = null;
        if (value is string text)
        {
            candidate = text;
        }
        else if (value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.String)
        {
            candidate = jsonElement.GetString();
        }
        bool isSvg = IsSvg(candidate);
        svg = isSvg ? candidate : null;
        return isSvg;
    }

    public static string ToDataUri(string svg)
    {
        return $"data:image/svg+xml;base64,{Convert.ToBase64String(Encoding.UTF8.GetBytes(svg))}";
    }
}
