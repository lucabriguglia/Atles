using System.Threading.Tasks;
using Atlas.Models.Admin.Site;

namespace Atlas.Client.Components.Admin
{
    public abstract class SettingsPage : AdminPageBase
    {
        protected SettingsPageModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<SettingsPageModel>("api/admin/sites/settings");
        }
        protected async Task UpdateAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/sites/update", Model);
            NavigationManager.NavigateTo("/admin/dashboard");
        }
    }
}