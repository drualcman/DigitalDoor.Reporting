namespace DigitalDoor.Reporting.Blazor.Components;

public partial class PrintReport
{
    [Inject] public GenerateReportAsPDF GeneratePDF { get; set; }
    [Parameter][EditorRequired] public ReportViewModel ReportModel { get; set; }
    [Parameter] public bool DirectDownload { get; set; } = true;
    [Parameter] public string PdfName { get; set; } = "document.pdf";
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
    [Parameter] public EventCallback BeginInvoke { get; set; }
    [Parameter] public EventCallback EndInvoke { get; set; }
    [Parameter] public EventCallback<PdfResponse> OnCreate { get; set; }
    [Parameter] public EventCallback<string> OnGetHtml { get; set; }

    string WrapperId = $"doc{Guid.NewGuid().ToString().Replace("-", "")}";
    string WrapperCss = "button is-primary btn btn-primary";

    protected override void OnParametersSet()
    {
        if (AdditionalAttributes is not null && AdditionalAttributes.TryGetValue("class", out object css)) WrapperCss = css.ToString();
        base.OnParametersSet();
    }

    async Task GeneratePdf(string pdfName)
    {
        if(BeginInvoke.HasDelegate)
            await BeginInvoke.InvokeAsync();

        PdfResponse response = new();
        response.Result = true;
        if(DirectDownload)
            response = await GeneratePDF.SaveAsFile(ReportModel, pdfName);
        else
            response = await GeneratePDF.GenerateReportAync(ReportModel);

        response.Html = await GeneratePDF.GetHtml(WrapperId);

        if(OnCreate.HasDelegate)
            await OnCreate.InvokeAsync(response);

        if(EndInvoke.HasDelegate)
            await EndInvoke.InvokeAsync();
    }

    Task GeneratePdf() =>
        GeneratePdf(string.IsNullOrEmpty(PdfName) ? "document.pdf" : PdfName);
}