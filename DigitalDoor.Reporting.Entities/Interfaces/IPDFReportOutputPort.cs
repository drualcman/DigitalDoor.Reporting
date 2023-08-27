namespace DigitalDoor.Reporting.Entities.Interfaces;

public interface IPDFReportOutputPort
{
    Task Handle(ReportViewModel report);
}
