namespace DigitalDoor.Reporting.Entities.Helpers;

public static class ReportPagination
{
    public const string TotalPagesPropertyName = "TotalPages";
    public const string CurrentPagePropertyName = "CurrentPage";
    private const double DivisionTolerance = 0.000001;

    public static bool IsPaginationItem(Item item)
    {
        return item is not null &&
            (item.PropertyName == TotalPagesPropertyName || item.PropertyName == CurrentPagePropertyName);
    }

    public static int GetRowsPerColumn(Section body, int totalBodyRows)
    {
        double bodyHeight = body.Format.Dimension.Height;
        double rowHeight = body.Row?.Dimension.Height ?? bodyHeight;
        int rowsPerColumn;
        if (rowHeight > 0)
        {
            rowsPerColumn = Math.Max(1, (int)Math.Floor(bodyHeight / rowHeight + DivisionTolerance));
        }
        else
        {
            rowsPerColumn = Math.Max(1, totalBodyRows);
        }
        return rowsPerColumn;
    }

    public static int GetColumnsPerPage(Section body)
    {
        return Math.Max(1, body.ColumnsNumber);
    }

    public static int GetTotalPages(Section body, int totalBodyRows)
    {
        int rowsPerPage = GetRowsPerColumn(body, totalBodyRows) * GetColumnsPerPage(body);
        return Math.Max(1, (int)Math.Ceiling((double)totalBodyRows / rowsPerPage));
    }

    public static int CountBodyRows(IEnumerable<ColumnData> data)
    {
        return data.Where(columnData => columnData.Section == SectionType.Body)
            .Select(columnData => columnData.Row)
            .Distinct()
            .Count();
    }
}
