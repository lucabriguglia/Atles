using Atlas.Client.Pages;
using Atlas.Client.Services;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atlas.Client.Components.Themes
{
    public abstract class ThemeComponentBase : ComponentBase
    {
        [CascadingParameter] protected CurrentMemberModel Member { get; set; }
        [CascadingParameter] protected CurrentSiteModel Site { get; set; }

        [Inject] public ApiService ApiService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStringLocalizer<PublicResources> Loc { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
    }
}