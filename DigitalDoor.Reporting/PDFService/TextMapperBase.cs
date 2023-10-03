using DigitalDoor.Reporting.Entities.ValueObjects;
using iText.Kernel.Colors;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;

namespace DigitalDoor.Reporting.PDFService;

internal class TextMapperBase : TextHelper
{
    public Color GetColor(string color)
    {
        System.Drawing.Color Color;
        if(color.Contains('#'))
        {
            Color = System.Drawing.ColorTranslator.FromHtml(color);
        }
        else
        {
            Color = System.Drawing.Color.FromName(color);
        }
        return new DeviceRgb(Color);
    }

    public double ConvertAngleToRadian(double angle)
    {
        return angle *(Math.PI/-180.0);
    }

    public void DrawBackground(Document page, string color, int positionPage, double heightBackground, decimal top)
    {
        if (color.ToLower() != "transparent" &&  !string.IsNullOrEmpty(color))
        {
            Div Background = new Div();
            Background.SetBackgroundColor(GetColor(color));
            Background.SetHeight(MillimeterToPixel(heightBackground));
            Background.SetPageNumber(positionPage);
            Background.SetFixedPosition(0, MillimeterToPixel(top), page.GetPdfDocument().GetDefaultPageSize().GetWidth());
            page.Add(Background);
        }
    }

    public iText.Layout.Borders.Border GetBorder(BorderStyle style, double width, string color)
    {
        return style switch
        {
            BorderStyle.dashed => new DashedBorder(GetColor(color), (float)width),
            BorderStyle.@double => new DoubleBorder(GetColor(color), (float)width),
            BorderStyle.groove => new GrooveBorder((DeviceRgb)GetColor(color), (float)width),
            BorderStyle.dotted => new DottedBorder(GetColor(color), (float)width),
            BorderStyle.outset => new OutsetBorder((DeviceRgb)GetColor(color), (float)width),
            BorderStyle.inset => new InsetBorder((DeviceRgb)GetColor(color), (float)width),
            BorderStyle.ridge => new RidgeBorder((DeviceRgb)GetColor(color), (float)width),
            BorderStyle.solid => new SolidBorder(GetColor(color), (float)width),
            _ => new SolidBorder(GetColor(color), 0)
        };
    }
}
