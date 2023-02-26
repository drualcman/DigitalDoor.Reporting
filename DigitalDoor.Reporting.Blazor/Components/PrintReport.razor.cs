﻿using DigitalDoor.Reporting.Blazor.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;
using DigitalDoor.Reporting.Entities.ViewModels;
using iText.Commons.Actions.Contexts;
using iText.Html2pdf;
using iText.Html2pdf.Attach;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Resolver.Resource;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;

namespace DigitalDoor.Reporting.Blazor.Components
{
    public partial class PrintReport
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
        string WrapperId = $"doc{Guid.NewGuid()}";

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
                    await using IJSObjectReference JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/DigitalDoor.Reporting.Blazor/Printing-Report.js");
                    await JSModule.InvokeVoidAsync("PrintReports.AddJavascriptsToPage", ReportModel.Page.Dimension.Width, ReportModel.Page.Dimension.Height);
                }
            }
        }

        async public Task GeneratePdf(string pdfName)
        {
            if(BeginInvoke.HasDelegate)
                await BeginInvoke.InvokeAsync();

            PdfResponse response = await DocumentBuilder.GetHtml();
            try
            {
                //string pageOrientation = ReportModel.Page.Orientation == Orientation.Landscape ? "l" : "p";

                //PageDimension = ReportModel.Page.Dimension;
                //ReportFunctions reportFunctions = new ReportFunctions();
                //string paperSize = reportFunctions.GetPaperSizeName(PageDimension).ToLower();
                //await using IJSObjectReference JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/DigitalDoor.Reporting.Blazor/Printing-Report.js");
                //response = await JSModule.InvokeAsync<PdfResponse>("PrintReports.CreatePdf", WrapperId, pdfName, !DirectDownload, pageOrientation, paperSize);

                response.Html += $"<link href=\"{NavigationManager.BaseUri}_content/DigitalDoor.Reporting.Blazor/DigitalDoor.Reporting.Blazor.bundle.scp.css\" rel=\"stylesheet\" type=\"text/css\" />";

                MemoryStream pdf = new MemoryStream(response.Html.Length);
                ConverterProperties properties = new ConverterProperties();
                properties.SetBaseUri(NavigationManager.BaseUri);
                properties.SetImmediateFlush(true);
                
                HtmlConverter.ConvertToPdf(response.Html, pdf, properties);
                
                byte[] bytes = pdf.ToArray();
                response.Base64String = Convert.ToBase64String(bytes);

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

class Resources : IResourceRetriever
{

    //_content/DigitalDoor.Reporting.Blazor/DigitalDoor.Reporting.Blazor.bundle.scp.css
    public byte[] GetByteArrayByUrl(Uri url) => throw new NotImplementedException();
    public Stream GetInputStreamByUrl(Uri url) => throw new NotImplementedException();
}