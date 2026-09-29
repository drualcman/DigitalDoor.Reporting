using System.Net;
using DigitalDoor.Reporting.Entities.Layout;

namespace DigitalDoor.Reporting.Entities.Helpers.Html;

internal static class HtmlCellWriter
{
    public static void WriteCells(StringBuilder html, IEnumerable<ReportLayoutCell> cells, double originLeft, double originTop, ReportBodyPage page)
    {
        foreach (ReportLayoutCell cell in cells)
        {
            WriteCell(html, cell, originLeft, originTop, page);
        }
    }

    public static void WriteBox(StringBuilder html, LayoutBox box, string background, Border borders)
    {
        string style = HtmlBoxStyles.Box(box) + HtmlBoxStyles.Background(background) + HtmlBoxStyles.Borders(borders);
        html.Append($"<div style=\"{WebUtility.HtmlEncode(style)}\"></div>");
    }

    private static void WriteCell(StringBuilder html, ReportLayoutCell cell, double originLeft, double originTop, ReportBodyPage page)
    {
        Format format = cell.Format;
        LayoutBox cellBox = ReportCellLayout.GetCellBox(format, originLeft, originTop);
        string imageSource = cell.Pagination == ReportCellPagination.None && cell.Value is not null ? HtmlImageSource.GetDataUri(cell.Value) : null;
        string text = imageSource is null ? cell.GetTextForPage(page.PageNumber, page.TotalPages) : null;
        string style = HtmlBoxStyles.Box(cellBox) + "overflow:hidden;" +
            HtmlBoxStyles.Background(format.Background) + HtmlBoxStyles.Borders(format.Borders) +
            HtmlBoxStyles.RotationAndOpacity(format) + (imageSource is null ? HtmlTextStyles.Text(format) : string.Empty);
        html.Append($"<div style=\"{WebUtility.HtmlEncode(style)}\">");
        if (imageSource is not null)
        {
            WriteImage(html, cell, cellBox, imageSource);
        }
        else if (!string.IsNullOrWhiteSpace(text))
        {
            html.Append(WebUtility.HtmlEncode(text));
        }
        html.Append("</div>");
    }

    private static void WriteImage(StringBuilder html, ReportLayoutCell cell, LayoutBox cellBox, string imageSource)
    {
        LayoutBox paddingBox = ReportCellLayout.GetPaddingBox(cellBox, cell.Format);
        LayoutBox contentBox = ReportCellLayout.GetContentBox(cellBox, cell.Format);
        string objectFit = cell.Value.Kind == ReportCellValueKind.SvgImage ? "contain" : "fill";
        string style = $"position:absolute;display:block;margin:0;padding:0;border:none;" +
            $"left:{CssValue.Millimeters(contentBox.Left - paddingBox.Left)};top:{CssValue.Millimeters(contentBox.Top - paddingBox.Top)};" +
            $"width:{CssValue.Millimeters(Math.Max(0, contentBox.Width))};height:{CssValue.Millimeters(Math.Max(0, contentBox.Height))};" +
            $"object-fit:{objectFit};";
        html.Append($"<img alt=\"\" style=\"{WebUtility.HtmlEncode(style)}\" src=\"{imageSource}\" />");
    }
}
