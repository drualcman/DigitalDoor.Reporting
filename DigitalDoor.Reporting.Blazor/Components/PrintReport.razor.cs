using DigitalDoor.Reporting.Entities.Interfaces;

namespace DigitalDoor.Reporting.Blazor.Components
{
    public partial class PrintReport
    {
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public IReportAsBytes ReportPdf { get; set; }

        [Parameter][EditorRequired] public ReportViewModel ReportModel { get; set; }
        [Parameter] public bool DirectDownload { get; set; } = true;
        [Parameter] public string PdfName { get; set; } = "document.pdf";
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
        [Parameter] public EventCallback BeginInvoke { get; set; }
        [Parameter] public EventCallback EndInvoke { get; set; }
        [Parameter] public EventCallback<PdfResponse> OnCreate { get; set; }
        [Parameter] public EventCallback<string> OnGetHtml { get; set; }

        ReportView DocumentBuilder;

        string WrapperId = $"doc{Guid.NewGuid().ToString().Replace("-", "")}";

        async Task GeneratePdf(string pdfName)
        {
            if(BeginInvoke.HasDelegate)
                await BeginInvoke.InvokeAsync();
            PdfResponse response;
            if(DirectDownload)
            {
                response = await DocumentBuilder.SaveAsFile(pdfName);
            }
            else
            {
                response = new();
                response.Html = await DocumentBuilder.GetHtml();
                byte[] report = await ReportPdf.GenerateReport(ReportModel);
                response.Base64String = Convert.ToBase64String(report);
            }

            if(OnCreate.HasDelegate)
                await OnCreate.InvokeAsync(response);

            if(EndInvoke.HasDelegate)
                await EndInvoke.InvokeAsync();
        }

        async public Task<string> GetHtml()
        {
            string result = await DocumentBuilder.GetHtml();            
            return result;
        }

        Task GeneratePdf() => 
            GeneratePdf(string.IsNullOrEmpty(PdfName) ? "document.pdf" : PdfName);
    }
}