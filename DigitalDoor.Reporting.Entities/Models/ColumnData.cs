using DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.Entities.Models
{
    public class ColumnData
    {
        public object Value{ get; set; }
        public Item Column { get; set; }
        public SectionType Section { get; set; }
        public ValueObjects.ValueType ValueType { get; set; } = ValueObjects.ValueType.Value;

        public int Row { get; set; } = 1;

        public Format? Format { get; set; } = null!;

        public ColumnData() { Section = SectionType.Body; }
        public ColumnData(Item column, object value) : this() =>
            (Value, Column) = (value, column);
        public ColumnData(Item column, object value, SectionType section) =>
            (Value, Column, Section) = (value, column, section);

    }
}
