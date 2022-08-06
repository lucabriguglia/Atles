using Atles.Client.Pages.Admin;
using Atles.Client.Services.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atles.Client.Components.Admin;

public abstract class AdminComponentBase : ComponentBase
{
    [Inject] public ApiService ApiService { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public IStringLocalizer<AdminResources> Localizer { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
}
