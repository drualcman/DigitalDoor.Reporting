namespace DigitalDoor.Reporting.Helpers;
public class RgbColors
{
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
    public int A { get; set; }

    public RgbColors(int r, int g, int b, int a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static RgbColors FromHex(string hex)
    {
        if (hex.StartsWith("#"))
        {
            hex = hex.Substring(1);
        }

        if (hex.Length == 8)
        {
            return new RgbColors(
                Convert.ToInt32(hex.Substring(0, 2), 16),
                Convert.ToInt32(hex.Substring(2, 2), 16),
                Convert.ToInt32(hex.Substring(4, 2), 16),
                Convert.ToInt32(hex.Substring(6, 2), 16)
            );
        }
        else if (hex.Length == 6)
        {
            return new RgbColors(
                Convert.ToInt32(hex.Substring(0, 2), 16),
                Convert.ToInt32(hex.Substring(2, 2), 16),
                Convert.ToInt32(hex.Substring(4, 2), 16)
            );
        }
        else if (hex.Length == 3)
        {
            return new RgbColors(
                Convert.ToInt32(hex[0].ToString() + hex[0].ToString(), 16),
                Convert.ToInt32(hex[1].ToString() + hex[1].ToString(), 16),
                Convert.ToInt32(hex[2].ToString() + hex[2].ToString(), 16)
            );
        }
        throw new ArgumentException("Invalid hex color format. Use #RRGGBB or #RGB or #RRGGBBAA.");
    }

    public static RgbColors FromName(string name)
    {
        try
        {
            return FromHex(ColorTranslator.ConvertToHexColor(name));
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Invalid color name.", ex);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Invalid color name.", ex);
        }
    }
}
