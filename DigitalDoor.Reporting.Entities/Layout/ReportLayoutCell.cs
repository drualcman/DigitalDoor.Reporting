using System.Globalization;

namespace DigitalDoor.Reporting.Entities.Layout;

public sealed class ReportLayoutCell
{
    public Format Format { get; }
    public ReportCellValue Value { get; }
    public ReportCellPagination Pagination { get; }
    public string ColumnDescription { get; }

    public ReportLayoutCell(Format format, ReportCellValue value, ReportCellPagination pagination, string columnDescription)
    {
        Format = format;
        Value = value;
        Pagination = pagination;
        ColumnDescription = columnDescription;
    }

    public string GetTextForPage(int currentPage, int totalPages)
    {
        string text = Pagination switch
        {
            ReportCellPagination.CurrentPage => currentPage.ToString(CultureInfo.CurrentCulture),
            ReportCellPagination.TotalPages => totalPages.ToString(CultureInfo.CurrentCulture),
            _ => Value?.Text
        };
        return text;
    }
}
