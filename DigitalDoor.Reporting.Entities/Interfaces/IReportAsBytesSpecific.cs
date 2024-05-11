namespace DigitalDoor.Reporting.Entities.Interfaces;
public interface IReportAsBytes
{
    Task<byte[]> GenerateReport(ReportViewModel reportModel);
}
