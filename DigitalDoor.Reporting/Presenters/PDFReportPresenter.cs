using DigitalDoor.Reporting.Entities.Interfaces;
using DigitalDoor.Reporting.Entities.ViewModels;
using DigitalDoor.Reporting.PDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalDoor.Reporting.Presenters
{
    public class PDFReportPresenter : IPDFReportPresenter, IPDFReportOutputPort
    {
        public byte[] Report { get; private set; }

        public async Task Handle(ReportViewModel report)
        {
            ITextPDF PDF = new(report);
            Report = await PDF.CreatePDFReport();
        }
    }
}
