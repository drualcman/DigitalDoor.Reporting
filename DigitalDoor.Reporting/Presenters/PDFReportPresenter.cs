using DigitalDoor.Reporting.Entities.Interfaces;
using DigitalDoor.Reporting.Entities.ViewModels;
using DigitalDoor.Reporting.Interfaces;
using DigitalDoor.Reporting.PDF;
using DigitalDoor.Reporting.Utilities;

namespace DigitalDoor.Reporting.Presenters;

internal class PDFReportPresenter : IPDFReportPresenter, IPDFReportOutputPort
{

    public PDFReportPresenter(IReportFont reportFont)
    {
        FontService.GetReportFont(reportFont);
    }
    public PDFReportPresenter()
    {

    }

    public byte[] Report { get; private set; }

    public async Task Handle(ReportViewModel report)
    {
        TextPDF PDF = new(report);
        Report = await PDF.CreatePDFReport();
        FontService.DisposeFont();
    }
}
