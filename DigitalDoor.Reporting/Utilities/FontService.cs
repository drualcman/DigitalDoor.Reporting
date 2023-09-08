using DigitalDoor.Reporting.Interfaces;

namespace DigitalDoor.Reporting.Utilities
{
    internal static class FontService 
    {
        public static IReportFont  ReportFont { get; private set; }

        public static void GetReportFont(IReportFont reportFont)
        {
            ReportFont = reportFont;    
        }

        public static void DisposeFont()
        {
            ReportFont = null;
        }
    }
}
