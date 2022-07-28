namespace DigitalDoor.Reporting.Entities.ValueObjects
{
    public class Shade
    {
        public double Width { get { return WidthBK; } set { WidthBK = value; } }
        private double WidthBK;
        public string Colour { get { return ColourBK; } set { ColourBK = value; } }
        private string ColourBK;
        public Shade()
        {
            WidthBK = 0;
            ColourBK = "black";
        }
        public Shade(double with) : this() => WidthBK = with;
        public Shade(double with, string color) : this() => (WidthBK, ColourBK) = (with, color);

    }
}
