namespace DigitalDoor.Reporting.Entities.ValueObjects;

public class FontStyle
{
    public bool Italic { get { return ItalicBK; } set { ItalicBK = value; } }
    private bool ItalicBK;
    public int Bold { get; set; } = 400;
    public bool IsBold => Bold > 400;
    public FontStyle() { }
    public FontStyle(int bold) => Bold = bold;
    public FontStyle(int bold, bool italic)
    {
        Bold = bold;
        ItalicBK = italic;
    }

    public FontStyle(bool italic) => Italic = italic;
}
