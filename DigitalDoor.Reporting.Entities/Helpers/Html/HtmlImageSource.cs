using DigitalDoor.Reporting.Entities.Layout;

namespace DigitalDoor.Reporting.Entities.Helpers.Html;

internal static class HtmlImageSource
{
    public static string GetDataUri(ReportCellValue value)
    {
        string dataUri = value.Kind switch
        {
            ReportCellValueKind.SvgImage => SvgValidator.ToDataUri(value.Svg),
            ReportCellValueKind.RasterImage => $"data:{GetRasterMimeType(value.RasterImage)};base64,{Convert.ToBase64String(value.RasterImage)}",
            _ => null
        };
        return dataUri;
    }

    private static string GetRasterMimeType(byte[] image)
    {
        string header = BitConverter.ToString(image, 0, Math.Min(4, image.Length)).Replace("-", "");
        string mimeType = "image/png";
        if (header.StartsWith("FFD8FF", StringComparison.Ordinal))
        {
            mimeType = "image/jpeg";
        }
        else if (header.StartsWith("4749", StringComparison.Ordinal))
        {
            mimeType = "image/gif";
        }
        else if (header.StartsWith("424D", StringComparison.Ordinal))
        {
            mimeType = "image/bmp";
        }
        else if (header.StartsWith("52494646", StringComparison.Ordinal))
        {
            mimeType = "image/webp";
        }
        return mimeType;
    }
}
