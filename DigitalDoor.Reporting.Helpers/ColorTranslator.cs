namespace DigitalDoor.Reporting.Helpers;
public static class ColorTranslator
{
    public static string ConvertToHexColor(string color)
    {
        string result = CssColors.BLACK;

        if (color.StartsWith('#'))
        {
            result = color;
        }
        else
        {
            color = color.Trim().ToLower();
            string toConvert = color;

            if (color.StartsWith("rgb(") && color.EndsWith(")"))
            {
                toConvert = ConvertRgbToHex(color);
            }
            else if (color.StartsWith("hsl(") && color.EndsWith(")"))
            {
                toConvert = ConvertHslToHex(color);
            }
            else if (color.StartsWith("hwb(") && color.EndsWith(")"))
            {
                toConvert = ConvertHwbToHex(color);
            }
            else
            {
            }
            result = CssColors.ColorToHex(toConvert);
        }
        return result;
    }

    private static string ConvertRgbToHex(string rgb)
    {
        string color = CssColors.BLACK;
        var match = Regex.Match(rgb, @"rgb\((\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3})\)");
        if (match.Success)
        {
            int r = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            int g = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            int b = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
            color = $"#{r:X2}{g:X2}{b:X2}";
        }
        return color;
    }

    private static string ConvertHslToHex(string hsl)
    {
        string color = CssColors.BLACK;
        var match = Regex.Match(hsl, @"hsl\((\d+),\s*(\d{1,3})%,\s*(\d{1,3})%\)");
        if (match.Success)
        {

            int h = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            double s = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture) / 100.0;
            double l = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture) / 100.0;

            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs(h / 60.0 % 2 - 1));
            double m = l - c / 2;

            double r = 0, g = 0, b = 0;

            switch (h)
            {
                case >= 0 and < 60:
                    r = c; g = x; b = 0;
                    break;
                case >= 60 and < 120:
                    r = x; g = c; b = 0;
                    break;
                case >= 120 and < 180:
                    r = 0; g = c; b = x;
                    break;
                case >= 180 and < 240:
                    r = 0; g = x; b = c;
                    break;
                case >= 240 and < 300:
                    r = x; g = 0; b = c;
                    break;
                default:
                    r = c; g = 0; b = x;
                    break;
            }

            color = $"#{(byte)((r + m) * 255):X2}{(byte)((g + m) * 255):X2}{(byte)((b + m) * 255):X2}";
        }
        return color;
    }

    private static string ConvertHwbToHex(string hwb)
    {
        string color = CssColors.BLACK;
        var match = Regex.Match(hwb, @"hwb\((\d+),\s*(\d{1,3})%,\s*(\d{1,3})%\)");
        if (match.Success)
        {

            int h = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            double w = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture) / 100.0;
            double b = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture) / 100.0;

            double ratio = 1 - w - b;
            if (ratio > 0)
            {

                double c = (1 - Math.Abs(2 * ratio - 1)) * w;
                double x = c * (1 - Math.Abs(h / 60.0 % 2 - 1));
                double m = w - c / 2;

                double r = 0, g = 0, bColor = 0;

                switch (h)
                {
                    case >= 0 and < 60:
                        r = c; g = x; bColor = 0;
                        break;
                    case >= 60 and < 120:
                        r = x; g = c; bColor = 0;
                        break;
                    case >= 120 and < 180:
                        r = 0; g = c; bColor = x;
                        break;
                    case >= 180 and < 240:
                        r = 0; g = x; bColor = c;
                        break;
                    case >= 240 and < 300:
                        r = x; g = 0; bColor = c;
                        break;
                    default:
                        r = c; g = 0; bColor = x;
                        break;
                }

                color = $"#{(byte)((r + m) * 255):X2}{(byte)((g + m) * 255):X2}{(byte)((bColor + m) * 255):X2}";
            }
        }
        return color;
    }
}
