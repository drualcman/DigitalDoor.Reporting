using DigitalDoor.Reporting.Entities.Interfaces;
using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;
using DigitalDoor.Reporting.Entities.ViewModels;

namespace DigitalDoor.Reporting.Presenters;

public class ReportsPresenter : IReportsPresenter, IReportsOutputPort
{
    public ReportViewModel Content { get; private set; }

    public Task Handle(Setup setup, List<ColumnData> data)
    {
        int rows = data.FindAll
            (x => x.Section == SectionType.Body).GroupBy(i => i.Row).Count();

        //int rows = data.GroupBy(i => i.Row).Count();
        int columns = setup.Body.ColumnsNumber;
        int myRows = rows / columns;

        double pageHeight = setup.Body.Format.Dimension.Height;

        double rowHeight = setup.Body.Row?.Dimension.Height ?? setup.Body.Format.Dimension.Height;

        double totalHeight = myRows * rowHeight;

        double pages = (totalHeight / pageHeight);

        if(pages % 2 > 0) pages++;

        Content = new ReportViewModel(setup, data);
        if(pages != 0)
            Content.Pages = (int)pages;
        else
            Content.Pages = 1;
        LookForImages();

        return Task.CompletedTask;
    }

    void LookForImages()
    {
        Helpers.Images i = new Helpers.Images();
        foreach(ColumnData item in Content.Data)
        {
            if(i.TryGetImageBytes(item.Value, out byte[] image))
                item.Value = image;
        }
    }
}
