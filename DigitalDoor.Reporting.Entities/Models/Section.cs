namespace DigitalDoor.Reporting.Entities.Models;

public class Section
{
    public Format Format { get; set; }

    public Row Row { get; set; }
    public int ColumnsNumber { get; set; }
    public int ColumnsSpace { get; set; }
    public List<ColumnSetup> Items { get; set; } = new List<ColumnSetup>();

    public Section()
    {
        Format = new Format(SectionType.Body) { Dimension = new Dimension(PageSize.A4) };
        Row = new Row(Format.Dimension);
        ColumnsNumber = 1;
    }

    public Section(SectionType section) : this()
    {
        Format = new Format(section);
        Row = new Row(Format.Dimension);
    }

    public Section(Format format) : this()
    {
        Format = format;
        Row = new Row(Format.Dimension);
    }

    public Section(Dimension dimension) : this(new Format(dimension)) { }
    public Section(Dimension dimension, SectionType section) : this(new Format(dimension) { Section = section }) { }
    public Section(Dimension dimension, Orientation orientation) : this(new Format(dimension))
    {
        if (orientation == Orientation.Landscape)
        {
            Row = new Row(Format.Dimension.Height, Format.Dimension.Width);
        }
    }

    public void AddColumn(ColumnSetup column)
    {
        column.Format.Section = Format.Section;
        Items.Add(column);
    }
    public void AddColumn(string name, Dimension dimension)
    {
        ColumnSetup column = new ColumnSetup
        {
            DataColumn = new Item(name),
            Format = new Format(dimension) { Section = this.Format.Section }
        };
        Items.Add(column);
    }
    public void AddColumn(string name, Format format)
    {
        format.Section = Format.Section;
        ColumnSetup column = new ColumnSetup
        {
            DataColumn = new Item(name),
            Format = format
        };
        Items.Add(column);
    }


    public void AddColumn<T>(Expression<Func<T, object>> property)
    {
        ColumnSetup column = new ColumnSetup
        {
            DataColumn = Item.GetItem<T>(property),
            Format = this.Format
        };
        Items.Add(column);
    }

    public void AddColumn<T>(Expression<Func<T, object>> property, Format format)
    {
        format.Section = this.Format.Section;
        ColumnSetup column = new ColumnSetup
        {
            DataColumn = Item.GetItem<T>(property),
            Format = format
        };
        Items.Add(column);
    }
    public void AddColumn<T>(Expression<Func<T, object>> property, Dimension dimension)
    {
        ColumnSetup column = new ColumnSetup
        {
            DataColumn = Item.GetItem<T>(property),
            Format = new Format(dimension) { Section = this.Format.Section }
        };
        Items.Add(column);
    }
    public void RemoveColumn(ColumnSetup column) => Items.Remove(column);

    private void AddPagination(Dimension dimensions, Kernel position, Font fontDetails, string paginationItem) =>
        Items.Add(new ColumnSetup
        {
            Format = new Format(new Dimension(dimensions))
            {
                Section = Format.Section,
                Position = position,
                FontDetails = fontDetails,

            },
            DataColumn = new Item(paginationItem)
        });

    private readonly Dimension DefaultDimensions = new Dimension(5, 10);
    private readonly Font DefaultFontDetails = new Font("Arial", 13, "Black");

    private Kernel DefaultPosition()
    {
        double top = Format.Dimension.Height - 5;
        double left = Format.Dimension.Width - 5;
        return new Kernel((int)top, (int)left);
    }

    public void AddTotalPages() =>
        AddPagination(DefaultDimensions, DefaultPosition(), DefaultFontDetails, "TotalPages");
    public void AddTotalPages(Format setUp) =>
        AddPagination(setUp.Dimension, setUp.Position, setUp.FontDetails, "TotalPages");

    public void AddCurentPage() =>
        AddPagination(DefaultDimensions, DefaultPosition(), DefaultFontDetails, "CurrentPage");

    public void AddCurentPage(Format setUp) =>
        AddPagination(setUp.Dimension, setUp.Position, setUp.FontDetails, "CurrentPage");

}
