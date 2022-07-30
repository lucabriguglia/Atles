using System;
using System.Threading.Tasks;
using Atles.Client.Services.Api;
using Atles.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Atles.Client.Pages
{
    public partial class MainLayoutComponent : LayoutComponentBase
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public ApiService ApiService { get; set; }

        protected CurrentUserModel User { get; set; }
        protected CurrentSiteModel Site { get; set; }

        protected override async Task OnInitializedAsync()
        {
            User = await ApiService.GetFromJsonAsync<CurrentUserModel>("api/public/current-user");
            Site = await ApiService.GetFromJsonAsync<CurrentSiteModel>("api/public/current-site");

            await JsRuntime.InvokeVoidAsync("atlas.interop.changePageTitle", Site.Title);

            if (!string.IsNullOrWhiteSpace(Site.HeadScript))
            {
                await JsRuntime.InvokeVoidAsync("atlas.interop.prependScriptToHead", Site.HeadScript);
            }
        }

        protected RenderFragment AddLayout(string name, RenderFragment body) => builder =>
        {
            var type = GetLayoutType(Site.Theme, name);

            if (type == null)
            {
                type = GetLayoutType("Default", name);
            }

            builder.OpenComponent(0, type);
            builder.AddAttribute(1, "Body", body);
            builder.CloseComponent();
        };

        private static Type GetLayoutType(string theme, string name) => Type.GetType($"Atles.Client.Themes.{theme}.{name}Layout, {typeof(Program).Assembly.FullName}");
    }
}