using System;
using System.Net.Http;
using System.Threading.Tasks;
using Atlify.Models.Public;
using Atlify.Client.Pages;
using Atlify.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atlify.Client.Components.Themes
{
    public abstract class ThemeComponentBase : ComponentBase
    {
        [CascadingParameter] protected CurrentMemberModel Member { get; set; }
        [CascadingParameter] protected CurrentSiteModel Site { get; set; }

        [Inject] public ApiService ApiService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStringLocalizer<PublicResources> Loc { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        public bool SavingData { get; set; }
        protected string CssClassDisabled => SavingData ? "disabled" : string.Empty;

        protected async Task<HttpResponseMessage> SaveDataAsync(Func<Task<HttpResponseMessage>> actionAsync)
        {
            SavingData = true;
            var response = await actionAsync();
            SavingData = false;
            return response;
        }
    }
}