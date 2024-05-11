namespace DigitalDoor.Reporting.Blazor.Components;

public partial class PrintReport
{
    [Inject] GenerateReportAsBytes GenerateBytes { get; set; }
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

    async Task GenerateBytesData(string pdfName)
    {
        if(BeginInvoke.HasDelegate)
            await BeginInvoke.InvokeAsync();
        await Task.Delay(1);

        PdfResponse response = new();
        response.Result = true;
        if(DirectDownload)
            response = await GenerateBytes.SaveAsFile(ReportModel, pdfName);
        else
            response = await GenerateBytes.GenerateReportAync(ReportModel);

        response.Html = await GenerateBytes.GetHtml(WrapperId);

        if(OnCreate.HasDelegate)
            await OnCreate.InvokeAsync(response);
        await Task.Delay(1);

        if (EndInvoke.HasDelegate)
            await EndInvoke.InvokeAsync();
        await Task.Delay(1);
    }

    Task GenerateBytesData() =>
        GenerateBytesData(string.IsNullOrEmpty(PdfName) ? "document.pdf" : PdfName);
}