using System.Reflection;

namespace DigitalDoor.Reporting.Blazor.Components
{
    public partial class PrintReport : IAsyncDisposable
    {
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

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
        IJSObjectReference JSModule;

        protected override void OnParametersSet()
        {
            if(!OnCreate.HasDelegate && DirectDownload == false) DirectDownload = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                if(ReportModel is not null)
                {
                    JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./{ContentHelper.ContentPath}/Printing-Report.js?v={DateTime.Today.ToFileTimeUtc()}");
                    await JSModule.InvokeVoidAsync("PrintReports.AddJavascriptsToPage", ReportModel.Page.Dimension.Width, ReportModel.Page.Dimension.Height);
                }
            }
        }

        async public Task GeneratePdf(string pdfName)
        {
            if(BeginInvoke.HasDelegate)
                await BeginInvoke.InvokeAsync();

            PdfResponse response = await DocumentBuilder.GetHtml();
            if(response.Result)
            {
                try
                {
                    if(JSModule is not null)
                    {
                        string pageOrientation = ReportModel.Page.Orientation == Orientation.Landscape ? "l" : "p";

                        PageDimension = ReportModel.Page.Dimension;
                        ReportFunctions reportFunctions = new ReportFunctions();
                        string paperSize = reportFunctions.GetPaperSizeName(PageDimension).ToLower();
                        response = await JSModule.InvokeAsync<PdfResponse>("PrintReports.CreatePdf", WrapperId, pdfName, !DirectDownload, pageOrientation, paperSize);
                    }
                    else
                    {
                        response.Message = "Javascript not load! Can't create a PDF.";
                        response.Result = false;
                    }
                }
                catch(Exception ex)
                {
                    response = new PdfResponse
                    {
                        Base64String = string.Empty,
                        Message = ex.Message,
                        Result = false
                    };
                }
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

        public async ValueTask DisposeAsync()
        {
            if(JSModule != null)
            {
                try
                {
                    await JSModule.DisposeAsync();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}