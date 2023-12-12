namespace DigitalDoor.Reporting.Blazor.Helpers;
internal static class LoadJsModuleHelper
{        
    internal static Task<IJSObjectReference> GetJSObjectReference(IJSRuntime jsRuntime, string filename) =>
        jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", $"./{ContentHelper.ContentPath}/{filename}?v={DateTime.Today.ToFileTimeUtc()}").AsTask();

}
