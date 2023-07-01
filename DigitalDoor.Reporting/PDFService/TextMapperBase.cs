using DigitalDoor.Reporting.PDF;
using iText.Kernel.Colors;

namespace DigitalDoor.Reporting.PDFService
{
    internal class TextMapperBase : TextHelper
    {
        public Color GetColor(string color)
        {
            return color switch
            {
                "green" => ColorConstants.GREEN,
                _ => ColorConstants.BLACK
            };
        }

    }
}
