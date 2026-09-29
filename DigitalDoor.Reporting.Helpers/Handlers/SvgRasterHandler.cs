using System.Xml;

namespace DigitalDoor.Reporting.Helpers.Handlers;

internal static class SvgRasterHandler
{
    public static byte[] SvgToPngOrNull(string svgText)
    {
        byte[] result = null;
        string escapedSvg = svgText.Replace("&lt;", "&amp;lt;");
        using SKSvg svgDocument = new SKSvg();
        bool isWellFormedSvg = true;
        try
        {
            svgDocument.FromSvg(escapedSvg);
        }
        catch (XmlException)
        {
            isWellFormedSvg = false;
        }
        if (isWellFormedSvg && svgDocument.Picture is not null)
        {
            result = SKPictureHandler.PictureToByteArray(svgDocument.Picture);
        }
        return result;
    }
}
