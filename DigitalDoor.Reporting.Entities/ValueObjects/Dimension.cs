namespace DigitalDoor.Reporting.Entities.ValueObjects
{
    public class Dimension
    {
        public double Height { get; set; } = 0;
        public double Width { get; set; } = 0;
        //public double Height { get { return HeightBK; } set { HeightBK = value; } }

        //private double HeightBK;
        //public double Width { get { return WidthBK; } set { WidthBK = value; } }
        //private double WidthBK;

        public Dimension(double width, double height)
        {
            this.Width = width;
            this.Height = height;

        }
        public Dimension(Dimension dimension)
        {
            this.Width = dimension.Width;
            this.Height = dimension.Height;


        }

        public Dimension()
        {

        }

    }
}
