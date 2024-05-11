namespace DigitalDoor.Reporting.Entities.Interfaces;
public interface IReportAsBytes<TImplementation>
{
    Task<byte[]> GenerateReport(ReportViewModel reportModel);
}
