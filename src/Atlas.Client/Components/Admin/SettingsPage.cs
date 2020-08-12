using System.Threading.Tasks;
using Atlas.Models.Admin.Site;
using Microsoft.JSInterop;

namespace Atlas.Client.Components.Admin
{
    public abstract class SettingsPage : AdminPageBase
    {
        protected SettingsPageModel Model { get; set; }

        private string CurrentLanguage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<SettingsPageModel>("api/admin/sites/settings");
            CurrentLanguage = Model.Site.Language;
        }
        protected async Task UpdateAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/sites/update", Model);

            var forceLoad = false;

            if (CurrentLanguage != Model.Site.Language)
            {
                //await JsRuntime.InvokeVoidAsync("blazorCulture.set", Model.Site.Language);
                forceLoad = true;
            }

            NavigationManager.NavigateTo("/admin/dashboard", forceLoad);
        }
    }
}