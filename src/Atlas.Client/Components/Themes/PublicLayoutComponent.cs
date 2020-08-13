using System.Threading.Tasks;
using Atlas.Client.Pages;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atlas.Client.Components.Themes
{
    public abstract class PublicLayoutComponent : LayoutComponentBase
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStringLocalizer<PublicResources> Loc { get; set; }

        [CascadingParameter] protected CurrentMemberModel Member { get; set; }
        [CascadingParameter] protected CurrentSiteModel Site { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("addCssFile", Site.CssPublic, Site.CssAdmin);
        }
    }
}