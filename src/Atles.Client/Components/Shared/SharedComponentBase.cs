using Atles.Client.Services.Api;
using Atles.Client.Shared;
using Atles.Reporting.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atles.Client.Components.Shared
{
    public abstract class SharedComponentBase : ComponentBase
    {
        [CascadingParameter] protected CurrentUserModel User { get; set; }
        [CascadingParameter] protected CurrentSiteModel Site { get; set; }

        [Inject] public ApiService ApiService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStringLocalizer<SharedResources> Loc { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
    }
}