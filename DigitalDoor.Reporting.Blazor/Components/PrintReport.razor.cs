using DigitalDoor.Reporting.Blazor.Models;
using DigitalDoor.Reporting.Entities.Helpers;
using DigitalDoor.Reporting.Entities.ValueObjects;
using DigitalDoor.Reporting.Entities.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace DigitalDoor.Reporting.Blazor.Components
{
    public partial class PrintReport : IDisposable
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


        Dimension PageDimension;
        string WrapperId = $"doc{Guid.NewGuid()}";

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
                    JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/DigitalDoor.Reporting.Blazor/Printing-Report.js");
                    await JSModule.InvokeVoidAsync("PrintReports.AddCssToPage", ReportModel.Page.Dimension.Width, ReportModel.Page.Dimension.Height);
                }
            }
        }

        async public Task GeneratePdf(string pdfName)
        {
            if(BeginInvoke.HasDelegate)
                await BeginInvoke.InvokeAsync();

            string pageOrientation = ReportModel.Page.Orientation == Orientation.Landscape ? "l" : "p";

            PageDimension = ReportModel.Page.Dimension;
            ReportFunctions reportFunctions = new ReportFunctions();
            string paperSize = reportFunctions.GetPaperSizeName(PageDimension).ToLower();

            PdfResponse response;
            try
            {
                response = await JSModule.InvokeAsync<PdfResponse>("PrintReports.CreatePdf", WrapperId, pdfName, !DirectDownload, pageOrientation, paperSize);
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
            if(EndInvoke.HasDelegate)
                await EndInvoke.InvokeAsync();

            if(OnCreate.HasDelegate)
                await OnCreate.InvokeAsync(response);
        }

        Task GeneratePdf() => GeneratePdf(string.IsNullOrEmpty(PdfName) ? "document.pdf" : PdfName);

        public async void Dispose()
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
