using System.Net;
using DigitalDoor.Reporting.Entities.Layout;

namespace DigitalDoor.Reporting.Entities.Helpers.Html;

internal sealed class HtmlPageWriter
{
    public const string PageCssClass = "dd-report-page";

    private readonly ReportViewModel Report;
    private readonly ReportPageLayout Layout;
    private readonly ReportLayoutContent Content;

    public HtmlPageWriter(ReportViewModel report, ReportPageLayout layout, ReportLayoutContent content)
    {
        Report = report;
        Layout = layout;
        Content = content;
    }

    public void WritePage(StringBuilder html, ReportBodyPage page, bool isLastPage)
    {
        string pageStyle = $"position:relative;box-sizing:border-box;overflow:hidden;padding:0;" +
            $"width:{CssValue.Millimeters(Layout.PageWidth)};height:{CssValue.Millimeters(Layout.PageHeight)};" +
            $"background-color:{(CssValue.IsTransparent(Report.Page?.Background) ? "white" : CssValue.Colour(Report.Page.Background))};" +
            (isLastPage ? string.Empty : "break-after:page;page-break-after:always;");
        html.Append($"<div class=\"{PageCssClass}\" style=\"{WebUtility.HtmlEncode(pageStyle)}\">");
        HtmlCellWriter.WriteBox(html, Layout.Header.Bounds, Report.Header?.Format?.Background, Report.Header?.Format?.Borders);
        HtmlCellWriter.WriteBox(html, Layout.Body.Bounds, Report.Body?.Format?.Background, Report.Body?.Format?.Borders);
        HtmlCellWriter.WriteBox(html, Layout.Footer.Bounds, Report.Footer?.Format?.Background, Report.Footer?.Format?.Borders);
        WriteSectionRowBorder(html, Report.Header, Layout.Header, Content.HeaderHasRows);
        HtmlCellWriter.WriteCells(html, Content.HeaderCells, Layout.Header.ContentLeft, Layout.Header.ContentTop, page);
        WriteBodyColumns(html, page);
        HtmlCellWriter.WriteCells(html, Content.BodyPaginationCells, Layout.Body.ContentLeft, Layout.Body.ContentTop, page);
        WriteSectionRowBorder(html, Report.Footer, Layout.Footer, Content.FooterHasRows);
        HtmlCellWriter.WriteCells(html, Content.FooterCells, Layout.Footer.ContentLeft, Layout.Footer.ContentTop, page);
        html.Append("</div>");
    }

    private void WriteBodyColumns(StringBuilder html, ReportBodyPage page)
    {
        Dimension rowDimension = ReportBodyPaginator.GetRowDimension(Report.Body);
        double columnStep = ReportBodyPaginator.GetColumnStep(Report.Body);
        for (int columnIndex = 0; columnIndex < page.Columns.Count; columnIndex++)
        {
            IReadOnlyList<ReportLayoutRow> columnRows = page.Columns[columnIndex];
            for (int rowIndex = 0; rowIndex < columnRows.Count; rowIndex++)
            {
                double rowLeft = Layout.Body.ContentLeft + columnIndex * columnStep;
                double rowTop = Layout.Body.ContentTop + rowIndex * rowDimension.Height;
                HtmlCellWriter.WriteBox(html, new LayoutBox(rowLeft, rowTop, rowDimension.Width, rowDimension.Height), null, Report.Body?.Row?.Borders);
                HtmlCellWriter.WriteCells(html, columnRows[rowIndex].Cells, rowLeft, rowTop, page);
            }
        }
    }

    private static void WriteSectionRowBorder(StringBuilder html, Section section, ReportSectionLayout sectionLayout, bool sectionHasRows)
    {
        if (sectionHasRows && section?.Row?.Dimension is not null)
        {
            LayoutBox rowBox = new LayoutBox(sectionLayout.ContentLeft, sectionLayout.ContentTop, section.Row.Dimension.Width, section.Row.Dimension.Height);
            HtmlCellWriter.WriteBox(html, rowBox, null, section.Row.Borders);
        }
    }
}
