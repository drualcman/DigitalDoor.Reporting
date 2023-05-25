using DigitalDoor.Reporting.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalDoor.Reporting.Entities.Interfaces
{
    public interface IPDFReportPresenter
    {
        public byte[] Report { get; }
    }
}
