using System;
using System.Threading.Tasks;
using Atlify.Models.Public;
using Atlify.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Atlify.Client.Components
{
    public abstract class MainLayout : LayoutComponentBase
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public ApiService ApiService { get; set; }

        protected CurrentMemberModel Member { get; set; }
        protected CurrentSiteModel Site { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Member = await ApiService.GetFromJsonAsync<CurrentMemberModel>("api/public/current-member");
            Site = await ApiService.GetFromJsonAsync<CurrentSiteModel>("api/public/current-site");
            await JsRuntime.InvokeVoidAsync("changePageTitle", Site.Title);
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

        private static Type GetLayoutType(string theme, string name) => Type.GetType($"Atlify.Client.Themes.{theme}.{name}Layout, {typeof(Program).Assembly.FullName}");
    }
}