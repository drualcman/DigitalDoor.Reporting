namespace DigitalDoor.Reporting.Presenters;

internal class ReportsPresenter : IReportsPresenter, IReportsOutputPort
{
    public ReportViewModel Content { get; private set; }

    public Task Handle(Setup setup, List<ColumnData> data)
    {
        int totalBodyRows = ReportPagination.CountBodyRows(data);
        AddMissingPaginationData(setup.Header, SectionType.Header, data);
        AddMissingPaginationData(setup.Footer, SectionType.Footer, data);
        Content = new ReportViewModel(setup, data);
        Content.Pages = ReportPagination.GetTotalPages(Content.Body, totalBodyRows);
        LookForImages();
        return Task.CompletedTask;
    }

    private static void AddMissingPaginationData(Section section, SectionType sectionType, List<ColumnData> data)
    {
        AddMissingPaginationItem(section, sectionType, ReportPagination.TotalPagesPropertyName, data);
        AddMissingPaginationItem(section, sectionType, ReportPagination.CurrentPagePropertyName, data);
    }

    private static void AddMissingPaginationItem(Section section, SectionType sectionType, string propertyName, List<ColumnData> data)
    {
        ColumnSetup paginationSetup = section.Items.FirstOrDefault(item => item.DataColumn.PropertyName == propertyName);
        bool alreadyHasData = data.Any(columnData => columnData.Section == sectionType &&
            columnData.Column is not null && columnData.Column.PropertyName == propertyName);
        if (paginationSetup is not null && !alreadyHasData)
        {
            List<int> sectionRows = data.Where(columnData => columnData.Section == sectionType)
                .Select(columnData => columnData.Row)
                .ToList();
            data.Add(new ColumnData
            {
                Section = sectionType,
                Column = paginationSetup.DataColumn,
                Row = sectionRows.Count > 0 ? sectionRows.Min() : 1,
                Value = 1
            });
        }
    }

    private void LookForImages()
    {
        Helpers.Images images = new Helpers.Images();
        foreach (ColumnData item in Content.Data)
        {
            if (images.TryGetSvg(item.Value, out string svg))
            {
                item.Value = svg;
            }
            else if (images.TryGetImageBytes(item.Value, out byte[] image))
            {
                item.Value = image;
            }
        }
    }
}
