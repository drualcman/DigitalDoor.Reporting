namespace DigitalDoor.Reporting.Entities.ValueObjects
{
    /// <summary>
    /// Dimentions between of objects
    /// </summary>
    public class Kernel
    {
        public decimal Top { get { return TopBK; } set { TopBK = value; } }
        private decimal TopBK;
        public decimal Right { get { return RightBK; } set { RightBK = value; } }
        private decimal RightBK;

        public decimal Bottom { get { return BottomBK; } set { BottomBK = value; } }
        private decimal BottomBK;
        public decimal Left { get { return LeftBK; } set { LeftBK = value; } }
        private decimal LeftBK;

        public Kernel()
        {
            TopBK = 0;
            RightBK = 0;
            BottomBK = 0;
            LeftBK = 0;

        }
        public Kernel(decimal allPoint) : this(allPoint, allPoint, allPoint, allPoint) { }
        public Kernel(decimal top, decimal left) : this(top, 0, 0, left) { }
        public Kernel(decimal top, decimal right, decimal bottom, decimal left) : this() =>
            (TopBK, RightBK, BottomBK, LeftBK) = (top, right, bottom, left);


    }
}
