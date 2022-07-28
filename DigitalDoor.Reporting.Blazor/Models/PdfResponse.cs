namespace DigitalDoor.Reporting.Blazor.Models
{
    public class PdfResponse
    {
        public bool Result { get; set; }
        public string Base64String { get; set; }
        public string Message { get; set; }
    }
}
