namespace DigitalDoor.Reporting.Entities.Layout;

public sealed class ReportBodyPage
{
    public int PageNumber { get; }
    public int TotalPages { get; }
    public IReadOnlyList<IReadOnlyList<ReportLayoutRow>> Columns { get; }

    public ReportBodyPage(int pageNumber, int totalPages, IReadOnlyList<IReadOnlyList<ReportLayoutRow>> columns)
    {
        PageNumber = pageNumber;
        TotalPages = totalPages;
        Columns = columns;
    }
}
