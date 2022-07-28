namespace DigitalDoor.Reporting.Entities.ValueObjects
{
    /// <summary>
    /// Dimentions between of objects
    /// </summary>
    public class Kernel
    {
        public int Top { get { return TopBK; } set { TopBK = value; } }
        private int TopBK;
        public int Right { get { return RightBK; } set { RightBK = value; } }
        private int RightBK;

        public int Bottom { get { return BottomBK; } set { BottomBK = value; } }
        private int BottomBK;
        public int Left { get { return LeftBK; } set { LeftBK = value; } }
        private int LeftBK;

        public Kernel()
        {
            TopBK = 0;
            RightBK = 0;
            BottomBK = 0;
            LeftBK = 0;

        }
        public Kernel(int allPoint) : this(allPoint, allPoint, allPoint, allPoint) { }
        public Kernel(int top, int left) : this(top, 0, 0, left) { }
        public Kernel(int top, int right, int bottom, int left) : this() =>
            (TopBK, RightBK, BottomBK, LeftBK) = (top, right, bottom, left);


    }
}
