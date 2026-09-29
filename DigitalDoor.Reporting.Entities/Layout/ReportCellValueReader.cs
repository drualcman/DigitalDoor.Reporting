using System.Globalization;

namespace DigitalDoor.Reporting.Entities.Layout;

public static class ReportCellValueReader
{
    private const string DataUriPrefix = "data:";
    private const string Base64Marker = ";base64,";
    private const string SvgMimeType = "image/svg+xml";
    private static readonly string[] OtherValueNamespaces = { "SkiaSharp" };

    public static ReportCellValue Read(object value)
    {
        object normalizedValue = value is JsonElement jsonElement ? NormalizeJsonElement(jsonElement) : value;
        ReportCellValue cellValue = normalizedValue switch
        {
            null => ReportCellValue.Empty(value),
            byte[] bytes => ReadBytes(normalizedValue, bytes),
            string text => ReadText(normalizedValue, text),
            _ when IsDrawingObject(normalizedValue) => ReportCellValue.FromOther(normalizedValue),
            IFormattable formattable => ReportCellValue.FromText(normalizedValue, formattable.ToString(null, CultureInfo.CurrentCulture)),
            _ => ReportCellValue.FromText(normalizedValue, normalizedValue.ToString())
        };
        return cellValue;
    }

    private static ReportCellValue ReadBytes(object sourceValue, byte[] bytes)
    {
        ReportCellValue cellValue = ReportCellValue.FromRasterImage(sourceValue, bytes);
        if (!ImageValidator.HasImageHeader(bytes) && bytes.Length > 0)
        {
            ReportCellValue textImage = ReadText(sourceValue, Encoding.UTF8.GetString(bytes));
            cellValue = textImage.Kind == ReportCellValueKind.Text ? cellValue : textImage;
        }
        return cellValue;
    }

    private static ReportCellValue ReadText(object sourceValue, string text)
    {
        string trimmedText = text.Trim();
        ReportCellValue cellValue = ReportCellValue.FromText(sourceValue, text);
        if (trimmedText.StartsWith(DataUriPrefix, StringComparison.OrdinalIgnoreCase) &&
            trimmedText.IndexOf(Base64Marker, StringComparison.OrdinalIgnoreCase) > 0)
        {
            cellValue = ReadDataUri(sourceValue, trimmedText);
        }
        else if (SvgValidator.IsSvg(trimmedText))
        {
            cellValue = ReportCellValue.FromSvg(sourceValue, trimmedText);
        }
        else if (ImageValidator.TryDecodeBase64Image(trimmedText, out byte[] decodedImage))
        {
            cellValue = ReportCellValue.FromRasterImage(sourceValue, decodedImage);
        }
        return cellValue;
    }

    private static ReportCellValue ReadDataUri(object sourceValue, string dataUri)
    {
        int base64MarkerIndex = dataUri.IndexOf(Base64Marker, StringComparison.OrdinalIgnoreCase);
        string mimeType = dataUri.Substring(DataUriPrefix.Length, base64MarkerIndex - DataUriPrefix.Length);
        byte[] payload = Convert.FromBase64String(dataUri.Substring(base64MarkerIndex + Base64Marker.Length));
        return mimeType.Equals(SvgMimeType, StringComparison.OrdinalIgnoreCase) ?
            ReportCellValue.FromSvg(sourceValue, Encoding.UTF8.GetString(payload)) :
            ReportCellValue.FromRasterImage(sourceValue, payload);
    }

    private static object NormalizeJsonElement(JsonElement jsonElement)
    {
        object normalizedValue = jsonElement.ValueKind switch
        {
            JsonValueKind.String => jsonElement.GetString(),
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            _ => jsonElement.GetRawText()
        };
        return normalizedValue;
    }

    private static bool IsDrawingObject(object value)
    {
        string valueNamespace = value.GetType().Namespace ?? string.Empty;
        return OtherValueNamespaces.Any(drawingNamespace => valueNamespace.StartsWith(drawingNamespace, StringComparison.Ordinal));
    }
}
