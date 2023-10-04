namespace DigitalDoor.Reporting.Entities.Models;

public class Setup
{
    public bool IsLabel_ToDelete { get; set; }
    public Format Page { get; set; }
    public Section Header { get; set; }
    public Section Body { get; set; }
    public Section Footer { get; set; }
    public Setup()
    {
        IsLabel_ToDelete = false;
        Page = new Format(PageSize.A4) { Section = SectionType.Page };
        Header = new Section(SectionType.Header);
        Body = new Section() { };
        Footer = new Section(SectionType.Footer);
    }
    public Setup(Dimension pageSize) : this()
    {
        Page.Dimension = pageSize;
        Body = new Section(pageSize);
    }
    public Setup(double height, double width) : this(new Dimension(height, width)) { }
    public Setup(string backgroundColor, Dimension pageSize) : this(pageSize) { Page.Background = backgroundColor; }
    public Setup(Dimension pageSize, Orientation orientation) : this(pageSize) 
    { 
        Page.Orientation = orientation;
        Body = new Section(pageSize, orientation);
    }
    public Setup(string backgroundColor, Dimension pageSize, Orientation orientation) : this(pageSize, orientation) { Page.Background = backgroundColor; }
    public Setup(Dimension pageSize, Kernel padding) : this(pageSize) { Page.Padding = padding; }

}
