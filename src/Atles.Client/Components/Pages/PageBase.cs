using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atles.Client.Pages;
using Atles.Client.Services.Api;
using Atles.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Atles.Client.Components.Pages;

public abstract class PageBase : ComponentBase
{
    [CascadingParameter] protected CurrentUserModel User { get; set; }
    [CascadingParameter] protected CurrentSiteModel Site { get; set; }

    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public ApiService ApiService { get; set; }
    [Inject] public IStringLocalizer<PublicResources> Loc { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Site == null)
        {
            Site = await ApiService.GetFromJsonAsync<CurrentSiteModel>("api/public/current-site");
            await JsRuntime.InvokeVoidAsync("atlas.interop.changePageTitle", Site.Title);
        }
    }

    protected RenderFragment AddComponent(string name, Dictionary<string, object> models = null) => builder =>
    {
        var type = GetType(Site.Theme, name);

        if (type == null)
        {
            type = GetType("Default", name);
        }

        builder.OpenComponent(0, type);

        if (models != null)
        {
            for (var i = 0; i < models.Count; i++)
            {
                var (key, value) = models.ElementAt(i);
                builder.AddAttribute(i + 1, key, value);
            }
        }
            
        builder.CloseComponent();
    };

    private static Type GetType(string theme, string name) => Type.GetType($"Atles.Client.Themes.{theme}.{name}, {typeof(Program).Assembly.FullName}");
}
