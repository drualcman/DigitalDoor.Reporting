using DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.Entities.Models
{
    public class Row
    {
        public Dimension Dimension { get; set; }
        public Border Borders { get; set; }

        public List<Setup> Rows { get; set; }

        public void AddRow(Setup row)
        {
            if(Rows is null) Rows = new List<Setup>();
            Rows.Add(row);
        }
        public void RemoveRow(Setup row) => Rows?.Remove(row);

        public Row()
        {
            Dimension = new Dimension();
            Borders = new Border();
            Rows = null!;
        }

        public Row(Dimension dimension) :
            this() => Dimension = dimension;

        public Row(double height, double width) :
            this(new Dimension(height, width))
        { }

        public Row(Border borders) : this() => Borders = borders;
        public Row(Dimension dimension, Border borders) : this(dimension) => Borders = borders;

        public Row(double height, double width, int allBordersWidth) :
            this(new Dimension(height, width)) =>
            Borders = new Border(new Shade(allBordersWidth), new Shade(allBordersWidth),
                new Shade(allBordersWidth), new Shade(allBordersWidth), BorderStyle.solid);
    }
}
