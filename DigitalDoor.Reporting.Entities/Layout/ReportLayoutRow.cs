namespace DigitalDoor.Reporting.Entities.Layout;

public sealed class ReportLayoutRow
{
    public IReadOnlyList<ReportLayoutCell> Cells { get; }

    public ReportLayoutRow(IReadOnlyList<ReportLayoutCell> cells)
    {
        Cells = cells;
    }
}
