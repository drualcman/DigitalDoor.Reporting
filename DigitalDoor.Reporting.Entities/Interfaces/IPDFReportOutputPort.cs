using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalDoor.Reporting.Entities.Interfaces
{
    public interface IPDFReportOutputPort
    {
        Task Handle(ReportViewModel report);
    }
}
