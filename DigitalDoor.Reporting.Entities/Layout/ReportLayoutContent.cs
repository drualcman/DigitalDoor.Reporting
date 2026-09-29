namespace DigitalDoor.Reporting.Entities.Layout;

public sealed class ReportLayoutContent
{
    public IReadOnlyList<ReportLayoutCell> HeaderCells { get; }
    public bool HeaderHasRows { get; }
    public IReadOnlyList<ReportLayoutRow> BodyRows { get; }
    public IReadOnlyList<ReportLayoutCell> BodyPaginationCells { get; }
    public IReadOnlyList<ReportLayoutCell> FooterCells { get; }
    public bool FooterHasRows { get; }

    public ReportLayoutContent(IReadOnlyList<ReportLayoutCell> headerCells, bool headerHasRows,
        IReadOnlyList<ReportLayoutRow> bodyRows, IReadOnlyList<ReportLayoutCell> bodyPaginationCells,
        IReadOnlyList<ReportLayoutCell> footerCells, bool footerHasRows)
    {
        HeaderCells = headerCells;
        HeaderHasRows = headerHasRows;
        BodyRows = bodyRows;
        BodyPaginationCells = bodyPaginationCells;
        FooterCells = footerCells;
        FooterHasRows = footerHasRows;
    }

    public IEnumerable<ReportLayoutCell> GetAllCells()
    {
        return HeaderCells
            .Concat(BodyRows.SelectMany(row => row.Cells))
            .Concat(BodyPaginationCells)
            .Concat(FooterCells);
    }
}
