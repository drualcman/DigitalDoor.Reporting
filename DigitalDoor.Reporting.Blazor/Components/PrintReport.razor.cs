using DigitalDoor.Reporting.Entities.Interfaces;
using System.Reflection;

namespace DigitalDoor.Reporting.Blazor.Components
{
    public partial class PrintReport
    {
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public IPDFReportOutputPort OutPortPDF { get; set; }
        [Inject] public IPDFReportPresenter PresenterPDF { get; set; }

        [Parameter][EditorRequired] public ReportViewModel ReportModel { get; set; }
        [Parameter] public bool DirectDownload { get; set; }
        [Parameter] public bool ShowButton { get; set; } = true;

        [Parameter] public string PdfName { get; set; } = "document.pdf";
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
        [Parameter] public EventCallback BeginInvoke { get; set; }
        [Parameter] public EventCallback EndInvoke { get; set; }
        [Parameter] public EventCallback<PdfResponse> OnCreate { get; set; }
        [Parameter] public EventCallback<PdfResponse> OnGetHtml { get; set; }

        ReportView DocumentBuilder;

        Dimension PageDimension;
        string WrapperId = $"doc{Guid.NewGuid().ToString().Replace("-", "")}";

        protected override void OnParametersSet()
        {
            if(!OnCreate.HasDelegate && DirectDownload == false) DirectDownload = true;
        }


        async public Task GeneratePdf(string pdfName)
        {
            if(BeginInvoke.HasDelegate)
                await BeginInvoke.InvokeAsync();
            await OutPortPDF.Handle(ReportModel);
            byte[] Report =  PresenterPDF.Report;
            PdfResponse response = await DocumentBuilder.GetHtml();  
            if (DirectDownload)
            {
                 response = await DocumentBuilder.SaveAsFile(Report, pdfName);
            }
            else
            {
                response.Base64String = Convert.ToBase64String(Report);
            }
            if(OnCreate.HasDelegate)
                await OnCreate.InvokeAsync(response);

            if(EndInvoke.HasDelegate)
                await EndInvoke.InvokeAsync();

        }

        async public Task<PdfResponse> GetHtml()
        {
            PdfResponse pdfResponse = await DocumentBuilder.GetHtml();
            if(OnGetHtml.HasDelegate)
                await OnGetHtml.InvokeAsync(pdfResponse);
            return pdfResponse;
        }

        Task GeneratePdf() => GeneratePdf(string.IsNullOrEmpty(PdfName) ? "document.pdf" : PdfName);
    }
}