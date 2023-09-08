using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalDoor.Reporting.Interfaces
{
    public interface IReportFont
    {
        byte[] GetFontBytesArray(string fontName);  
    }
}
