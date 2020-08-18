using System.Threading.Tasks;
using Atlify.Models.Public;
using Atlify.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atlify.Client.Components.Themes
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