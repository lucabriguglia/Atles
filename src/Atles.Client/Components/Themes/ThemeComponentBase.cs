using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Atles.Client.Pages;
using Atles.Client.Services.Api;
using Atles.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atles.Client.Components.Themes
{
    public abstract class ThemeComponentBase : ComponentBase
    {
        [CascadingParameter] protected CurrentUserModel User { get; set; }
        [CascadingParameter] protected CurrentSiteModel Site { get; set; }

        [Inject] public ApiService ApiService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStringLocalizer<PublicResources> Loc { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public bool SavingData { get; set; }
        protected string CssClassDisabled => SavingData ? "disabled" : string.Empty;

        protected async Task<HttpResponseMessage> SaveDataAsync(Func<Task<HttpResponseMessage>> actionAsync)
        {
            SavingData = true;
            var response = await actionAsync();
            SavingData = false;
            return response;
        }

        protected async Task<ClaimsPrincipal> GetClaimsPrincipal()
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            return user;
        }
    }
}