namespace DigitalDoor.Reporting.Blazor.Services;
public class GenerateReportAsPDF : IAsyncDisposable
{
    readonly Lazy<Task<IJSObjectReference>> ModuleTask;
    readonly IReportAsBytes ReportPdf;

    public GenerateReportAsPDF(IJSRuntime JsRuntime, IReportAsBytes reportPdf)
    {
        ModuleTask = new Lazy<Task<IJSObjectReference>>(() => LoadJsModuleHelper.GetJSObjectReference(JsRuntime, "ReportTools.js"));
        ReportPdf = reportPdf;
    }

    public async Task<PdfResponse> GenerateReportAync(ReportViewModel reportModel)
    {
        PdfResponse response = new();
        byte[] report = await ReportPdf.GenerateReport(reportModel);
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
