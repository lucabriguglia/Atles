using Atlify.Client.Pages.Admin;
using Atlify.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atlify.Client.Components.Admin
{
    public abstract class AdminComponentBase : ComponentBase
    {
        [Inject] public ApiService ApiService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IStringLocalizer<AdminResources> Loc { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
    }
}