namespace DigitalDoor.Reporting.Entities.Layout;

public static class ReportBodyPaginator
{
    public static List<ReportBodyPage> Paginate(Section body, IReadOnlyList<ReportLayoutRow> bodyRows)
    {
        int rowsPerColumn = ReportPagination.GetRowsPerColumn(body, bodyRows.Count);
        int columnsPerPage = ReportPagination.GetColumnsPerPage(body);
        int totalPages = ReportPagination.GetTotalPages(body, bodyRows.Count);
        List<ReportBodyPage> pages = new List<ReportBodyPage>();
        for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
        {
            List<IReadOnlyList<ReportLayoutRow>> columns = new List<IReadOnlyList<ReportLayoutRow>>();
            int firstRowOfPage = pageIndex * rowsPerColumn * columnsPerPage;
            int columnIndex = 0;
            while (columnIndex < columnsPerPage && firstRowOfPage + columnIndex * rowsPerColumn < bodyRows.Count)
            {
                int firstRowOfColumn = firstRowOfPage + columnIndex * rowsPerColumn;
                int rowsInColumn = Math.Min(rowsPerColumn, bodyRows.Count - firstRowOfColumn);
                columns.Add(bodyRows.Skip(firstRowOfColumn).Take(rowsInColumn).ToList());
                columnIndex++;
            }
            pages.Add(new ReportBodyPage(pageIndex + 1, totalPages, columns));
        }
        return pages;
    }

    public static double GetColumnStep(Section body)
    {
        Dimension rowDimension = body.Row?.Dimension ?? body.Format.Dimension;
        return rowDimension.Width + body.ColumnsSpace;
    }

    public static Dimension GetRowDimension(Section body)
    {
        return body.Row?.Dimension ?? body.Format.Dimension;
    }
}
