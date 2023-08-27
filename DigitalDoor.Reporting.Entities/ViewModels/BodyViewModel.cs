namespace DigitalDoor.Reporting.Entities.ViewModels;

public class BodyViewModel
{
    public Format Format { get; set; }
    public BodyViewModel() : this(new Format(PageSize.A4)) { }
    public BodyViewModel(Format format)
    {
        Format = format;
    }
}
