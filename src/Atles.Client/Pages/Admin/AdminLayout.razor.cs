using System.Threading.Tasks;
using Atles.Client.Services.Api;
using Atles.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atles.Client.Pages.Admin
{
    public abstract class AdminLayoutComponent : LayoutComponentBase
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public SignOutSessionStateManager SignOutManager { get; set; }
        [Inject] public IStringLocalizer<AdminResources> Loc { get; set; }
        [Inject] public ApiService ApiService { get; set; }

        protected CurrentUserModel User { get; set; }
        protected CurrentSiteModel Site { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var site = await ApiService.GetFromJsonAsync<CurrentSiteModel>("api/public/current-site");
            await JsRuntime.InvokeVoidAsync("atlas.interop.addCssFile", site.CssAdmin, site.CssPublic);
        }

        protected void NavigateToAccount(MouseEventArgs args)
        {
            Navigation.NavigateTo("authentication/profile");
        }

        protected async Task BeginSignOut(MouseEventArgs args)
        {
            await SignOutManager.SetSignOutState();
            Navigation.NavigateTo("authentication/logout");
        }
    }
}