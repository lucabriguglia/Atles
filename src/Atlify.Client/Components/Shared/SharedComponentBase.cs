using Atlify.Models.Public;
using Atlify.Client.Services;
using Atlify.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atlify.Client.Components.Shared
{
    public abstract class SharedComponentBase : ComponentBase
    {
        [CascadingParameter] protected CurrentMemberModel Member { get; set; }
        [CascadingParameter] protected CurrentSiteModel Site { get; set; }

        [Inject] public ApiService ApiService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStringLocalizer<SharedResources> Loc { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
    }
}