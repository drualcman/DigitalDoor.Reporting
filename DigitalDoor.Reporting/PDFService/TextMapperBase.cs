using DigitalDoor.Reporting.Entities.ValueObjects;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using Org.BouncyCastle.Utilities.Encoders;

namespace DigitalDoor.Reporting.PDFService
{
    internal class TextMapperBase : TextHelper
    {
        public Color GetColor(string color)
        {
            System.Drawing.Color Color;
            if(color.Contains('#'))
            {
                Color  = System.Drawing.ColorTranslator.FromHtml(color);
            }
            else
            {
                Color = System.Drawing.Color.FromName(color);
            }
            return new DeviceRgb(Color);
        }

        public iText.Layout.Borders.Border GetBorder(BorderStyle style, double width)
        {
            return style switch
            {
                BorderStyle.dashed => new DashedBorder(ColorConstants.BLACK, (float)width),
                BorderStyle.@double => new DoubleBorder(ColorConstants.BLACK, (float)width),
                BorderStyle.groove => new GrooveBorder((DeviceRgb)ColorConstants.BLACK, (float)width),
                BorderStyle.dotted => new DottedBorder(ColorConstants.BLACK, (float)width),
                BorderStyle.outset => new OutsetBorder((DeviceRgb)ColorConstants.BLACK, (float)width),
                BorderStyle.inset => new InsetBorder((DeviceRgb)ColorConstants.BLACK, (float)width),
                BorderStyle.ridge => new RidgeBorder((DeviceRgb)ColorConstants.BLACK, (float)width),
                BorderStyle.solid  => new SolidBorder(ColorConstants.BLACK, (float)width),
                _ => new SolidBorder(ColorConstants.BLACK,0)
            };
        }
    }
}
