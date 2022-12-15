using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DigitalDoor.Reporting.Website.Pages
{
    public partial class Wiki
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        IJSObjectReference JSObjectReference { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstTime)
        {
            JSObjectReference = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/js/app.js");
            await JSObjectReference.InvokeVoidAsync("TabsSlide");
        }
    }
}
