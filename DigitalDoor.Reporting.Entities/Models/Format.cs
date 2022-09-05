using DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.Entities.Models
{
    public class Format
    {
        public Orientation Orientation { get; set; }
        public Kernel Padding { get; set; }
        public Dimension Dimension { get; set; }
        public Kernel Margin { get; set; }
        public Border Borders { get; set; }
        public string Background { get; set; }
        public double Angle { get; set; }
        public Kernel Position { get; set; }
        public Font FontDetails { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public TextDecoration TextDecoration { get; set; } = TextDecoration.None;
        public int Foreground { get; set; }
        public SectionType Section { get; set; }

        public Format() :
            this(0, 0)
        { }

        public Format(Format setup)
        {
            Orientation = setup.Orientation;
            Padding = setup.Padding;
            Dimension = setup.Dimension;
            Margin = setup.Margin;
            Borders = setup.Borders;
            Background = setup.Background;
            Position = setup.Position;
            FontDetails = setup.FontDetails;
            TextAlignment = setup.TextAlignment;
            TextDecoration = setup.TextDecoration;
            Foreground = setup.Foreground;
            Section = setup.Section;
        }

        public Format(SectionType section) : this() => Section = section;

        public Format(double width, double height) :
            this(new Dimension(width, height))
        { }

        public Format(Dimension dimension)
        {
            Dimension = new Dimension(dimension);
            Orientation = Orientation.Portrait;
            Padding = new Kernel();
            Margin = new Kernel();
            Borders = new Border();
            Background = "transparent";
            Position = new Kernel();
            FontDetails = new();
            Foreground = 0;
            Section = SectionType.Body;
        }
    }
}
