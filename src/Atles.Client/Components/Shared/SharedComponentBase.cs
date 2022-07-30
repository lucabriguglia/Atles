using System.Security.Claims;
using System.Threading.Tasks;
using Atles.Client.Services.Api;
using Atles.Client.Shared;
using Atles.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected async Task<ClaimsPrincipal> GetClaimsPrincipal()
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            return user;
        }
    }
}