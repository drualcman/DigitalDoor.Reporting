using DigitalDoor.Reporting.Entities.Models;

namespace DigitalDoor.Reporting.Entities.Interfaces
{
    public interface IReportDataRepository<TData>
    {
        List<ColumnData> GetReportData(List<TData> data, List<Format> columns);
    }
}
