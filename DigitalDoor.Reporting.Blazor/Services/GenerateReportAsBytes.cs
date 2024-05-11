namespace DigitalDoor.Reporting.Blazor.Services;
internal class GenerateReportAsBytes : IAsyncDisposable
{
    readonly Lazy<Task<IJSObjectReference>> ModuleTask;
    readonly IReportAsBytes ReportBytes;

    public GenerateReportAsBytes(IJSRuntime JsRuntime, IReportAsBytes reportPdf)
    {
        ModuleTask = new Lazy<Task<IJSObjectReference>>(() => LoadJsModuleHelper.GetJSObjectReference(JsRuntime, "ReportTools.js"));
        ReportBytes = reportPdf;
    }

    public async Task<PdfResponse> GenerateReportAync(ReportViewModel reportModel)
    {
        PdfResponse response = new();
        byte[] report = await ReportBytes.GenerateReport(reportModel);
        if(report.Length > 0)
        {
            response.Result = true;
            response.Base64String = Convert.ToBase64String(report);
        }
        return response;
    }

    public async Task<PdfResponse> SaveAsFile(ReportViewModel reportModel, string pdfName)
    {
        PdfResponse response = await GenerateReportAync(reportModel);
        if(response.Result)
        {
            try
            {
                IJSObjectReference module = await ModuleTask.Value;
                await module.InvokeVoidAsync("PrintReports.SaveAsFile", pdfName, response.Base64String);
            }
            catch(Exception ex)
            {;
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
        return response;
    }

    public async Task<string> GetHtml(string WrapperId)
    {
        string result;
        try
        {
            IJSObjectReference module = await ModuleTask.Value;
            PdfResponse response = await module.InvokeAsync<PdfResponse>("PrintReports.GetHtml", WrapperId);
            result = response.Html;
        }
        catch(Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
            result = string.Empty;
        }
        return result;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if(ModuleTask.IsValueCreated)
            {
                IJSObjectReference module = await ModuleTask.Value;
                await module.DisposeAsync();
            }
        }
        catch(Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
        }
    }
}
