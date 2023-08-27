using DigitalDoor.Reporting.Entities.Interfaces;
using DigitalDoor.Reporting.Entities.ViewModels;
using DigitalDoor.Reporting.PDF;

namespace DigitalDoor.Reporting.Presenters;

internal class PDFReportPresenter : IPDFReportPresenter, IPDFReportOutputPort
{
    public byte[] Report { get; private set; }

    public async Task Handle(ReportViewModel report)
    {
        TextPDF PDF = new(report);
        Report = await PDF.CreatePDFReport();
    }
}
