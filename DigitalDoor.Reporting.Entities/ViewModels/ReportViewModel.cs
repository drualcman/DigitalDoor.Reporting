namespace DigitalDoor.Reporting.Entities.ViewModels;

public class ReportViewModel : Setup
{

    #region properties
    public IEnumerable<ColumnData> Data { get; set; }
    public int Pages { get; set; } = 1;
    public int CurrentPage { get; set; } = 1;

    public ReportViewModel() : this(new Setup()) { }

    public ReportViewModel(IEnumerable<ColumnData> data) : this(new Setup(), data) { }

    public ReportViewModel(Setup setup)
    {
        Page = setup.Page;
        Header = setup.Header;
        if(Header.Row is null) Header.Row = new Row(Header.Format.Dimension);
        Body = setup.Body;
        if(Body.Row is null) Body.Row = new Row(Body.Format.Dimension);
        Footer = setup.Footer;
        if(Footer.Row is null) Footer.Row = new Row(Footer.Format.Dimension);
        Data = new List<ColumnData>();
    }
    public ReportViewModel(Setup setup, IEnumerable<ColumnData> data) : this(setup) => Data = data;

    public ColumnData GetColumnData(Item item) =>
        Data.FirstOrDefault(c => c.Column.ObjectName.ToLower() == item.ObjectName.ToLower() && c.Column.PropertyName.ToLower() == item.PropertyName.ToLower());
    public ColumnData GetColumn(string objectName, string propertyName) =>
        GetColumnData(new Item(objectName, propertyName));
    #endregion 
}
