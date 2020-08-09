using System.Threading.Tasks;
using Atlas.Client.Services;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Atlas.Client.Components
{
    public abstract class LayoutBase : LayoutComponentBase
    {
        [Inject] 
        public IJSRuntime JsRuntime { get; set; }

        [Inject] 
        public ApiService ApiService { get; set; }

        protected CurrentSiteModel Site { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Site = await ApiService.GetFromJsonAsync<CurrentSiteModel>("api/public/current-site");
            await JsRuntime.InvokeVoidAsync("changePageTitle", Site.Title);
            await JsRuntime.InvokeVoidAsync("addCssFile", Site.CssPublic, Site.CssAdmin);
        }

        //protected RenderFragment AddLayout(string layout) => builder =>
        //{
        //    var type = Type.GetType($"Atlas.Client.Themes.{Site.Theme}.{layout}, {typeof(Program).Assembly.FullName}");

        //    builder.OpenComponent(0, type);
        //    builder.CloseComponent();
        //};
    }
}