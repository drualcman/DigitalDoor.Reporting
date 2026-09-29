namespace DigitalDoor.Reporting.Entities.Layout;

public sealed class ReportCellValue
{
    public ReportCellValueKind Kind { get; }
    public object SourceValue { get; }
    public string Text { get; }
    public byte[] RasterImage { get; }
    public string Svg { get; }

    private ReportCellValue(ReportCellValueKind kind, object sourceValue, string text, byte[] rasterImage, string svg)
    {
        Kind = kind;
        SourceValue = sourceValue;
        Text = text;
        RasterImage = rasterImage;
        Svg = svg;
    }

    public static ReportCellValue Empty(object sourceValue) =>
        new ReportCellValue(ReportCellValueKind.Empty, sourceValue, null, null, null);

    public static ReportCellValue FromText(object sourceValue, string text) =>
        new ReportCellValue(ReportCellValueKind.Text, sourceValue, text, null, null);

    public static ReportCellValue FromRasterImage(object sourceValue, byte[] rasterImage) =>
        new ReportCellValue(ReportCellValueKind.RasterImage, sourceValue, null, rasterImage, null);

    public static ReportCellValue FromSvg(object sourceValue, string svg) =>
        new ReportCellValue(ReportCellValueKind.SvgImage, sourceValue, null, null, svg);

    public static ReportCellValue FromOther(object sourceValue) =>
        new ReportCellValue(ReportCellValueKind.Other, sourceValue, null, null, null);
}
