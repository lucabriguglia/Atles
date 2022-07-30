using System.Threading.Tasks;
using Atles.Client.Pages;
using Atles.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atles.Client.Components.Themes
{
    public abstract class PublicLayoutComponent : LayoutComponentBase
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStringLocalizer<PublicResources> Loc { get; set; }

        [CascadingParameter] protected CurrentUserModel User { get; set; }
        [CascadingParameter] protected CurrentSiteModel Site { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await JsRuntime.InvokeVoidAsync("atlas.interop.addCssFile", Site.CssPublic, Site.CssAdmin);
        }
    }
}