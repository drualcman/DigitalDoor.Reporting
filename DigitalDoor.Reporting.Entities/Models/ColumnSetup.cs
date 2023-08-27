namespace DigitalDoor.Reporting.Entities.Models;

public class ColumnSetup
{
    public Format Format { get; set; }
    public Item DataColumn { get; set; }
    public bool GroupedHeader { get; set; }
    public ColumnSetup() :
        this(new Format())
    { }

    public ColumnSetup(Format format)
    {
        Format = format;
        DataColumn = new Item();
        GroupedHeader = false;
    }
    public ColumnSetup(Format format, Item column)
    {
        Format = format;
        DataColumn = column;
        GroupedHeader = false;
    }
    public ColumnSetup(Dimension dimension) :
        this(new Format(dimension))
    { }
    public ColumnSetup(Dimension dimension, Item column) :
        this(new Format(dimension), column)
    { }
    public ColumnSetup(double width, double height) :
        this(new Dimension(width, height))
    { }

    public ColumnSetup(double width, double height, Item column) :
        this(new Dimension(width, height), column)
    { }
}
