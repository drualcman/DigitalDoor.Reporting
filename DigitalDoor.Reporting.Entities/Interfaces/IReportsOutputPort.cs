namespace DigitalDoor.Reporting.Entities.Interfaces;

public interface IReportsOutputPort
{
    Task Handle(Setup setup, List<ColumnData> data);
}
