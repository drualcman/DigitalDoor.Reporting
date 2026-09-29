namespace DigitalDoor.Reporting.Entities.Layout;

public static class ReportLayoutContentBuilder
{
    public static ReportLayoutContent Build(ReportViewModel report)
    {
        List<ColumnData> data = (report.Data ?? Enumerable.Empty<ColumnData>())
            .Where(columnData => columnData is not null && columnData.Column is not null)
            .ToList();
        List<ReportLayoutRow> headerRows = BuildRows(report.Header?.Items, data, SectionType.Header);
        List<ReportLayoutRow> bodyRows = BuildRows(report.Body?.Items, data, SectionType.Body);
        List<ReportLayoutRow> footerRows = BuildRows(report.Footer?.Items, data, SectionType.Footer);
        return new ReportLayoutContent(
            FlattenSection(headerRows, BuildPaginationCells(report.Header?.Items)), headerRows.Count > 0,
            bodyRows, BuildPaginationCells(report.Body?.Items),
            FlattenSection(footerRows, BuildPaginationCells(report.Footer?.Items)), footerRows.Count > 0);
    }

    private static List<ReportLayoutRow> BuildRows(List<ColumnSetup> setups, List<ColumnData> data, SectionType section)
    {
        List<ColumnSetup> dataSetups = (setups ?? new List<ColumnSetup>())
            .Where(setup => setup is not null && setup.DataColumn is not null && !ReportPagination.IsPaginationItem(setup.DataColumn))
            .ToList();
        return data.Where(columnData => columnData.Section == section)
            .GroupBy(columnData => columnData.Row)
            .OrderBy(rowGroup => rowGroup.Key)
            .Select(rowGroup => BuildRow(dataSetups, rowGroup.ToList()))
            .Where(row => row.Cells.Count > 0)
            .ToList();
    }

    private static ReportLayoutRow BuildRow(List<ColumnSetup> dataSetups, List<ColumnData> rowData)
    {
        List<ReportLayoutCell> cells = new List<ReportLayoutCell>();
        foreach (ColumnSetup setup in dataSetups)
        {
            ColumnData cellData = rowData.FirstOrDefault(columnData => setup.DataColumn.Equals(columnData.Column));
            Format cellFormat = cellData?.Format ?? setup.Format;
            if (cellData is not null && cellFormat is not null)
            {
                string columnDescription = $"{cellFormat.Section}.{setup.DataColumn.ObjectName}.{setup.DataColumn.PropertyName} (row {cellData.Row})";
                cells.Add(new ReportLayoutCell(cellFormat, ReportCellValueReader.Read(cellData.Value), ReportCellPagination.None, columnDescription));
            }
        }
        return new ReportLayoutRow(SortByForeground(cells));
    }

    private static List<ReportLayoutCell> BuildPaginationCells(List<ColumnSetup> setups)
    {
        return (setups ?? new List<ColumnSetup>())
            .Where(setup => setup is not null && setup.Format is not null && ReportPagination.IsPaginationItem(setup.DataColumn))
            .Select(setup => new ReportLayoutCell(setup.Format, ReportCellValue.Empty(null), GetPagination(setup.DataColumn), setup.DataColumn.PropertyName))
            .ToList();
    }

    private static List<ReportLayoutCell> FlattenSection(List<ReportLayoutRow> rows, List<ReportLayoutCell> paginationCells)
    {
        return SortByForeground(rows.SelectMany(row => row.Cells).Concat(paginationCells));
    }

    private static List<ReportLayoutCell> SortByForeground(IEnumerable<ReportLayoutCell> cells)
    {
        return cells.OrderBy(cell => cell.Format.Foreground).ToList();
    }

    private static ReportCellPagination GetPagination(Item paginationItem)
    {
        return paginationItem.PropertyName == ReportPagination.CurrentPagePropertyName ?
            ReportCellPagination.CurrentPage : ReportCellPagination.TotalPages;
    }
}
