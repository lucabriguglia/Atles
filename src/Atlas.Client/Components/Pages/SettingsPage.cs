using System.Threading.Tasks;
using Atlas.Models.Public;

namespace Atlas.Client.Components.Pages
{
    public abstract class SettingsPage : PageBase
    {
        protected SettingsPageModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Model = await ApiService.GetFromJsonAsync<SettingsPageModel>("api/public/settings/edit");
        }
    }
}