using System.Threading.Tasks;
using Atlas.Models.Admin.PermissionSets;

namespace Atlas.Client.Components.Admin.PermissionSets
{
    public abstract class CreatePage : AdminPageBase
    {
        protected FormComponentModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<FormComponentModel>("api/admin/permission-sets/create");
        }

        protected async Task SaveAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/permission-sets/save", Model.PermissionSet);
            NavigationManager.NavigateTo("/admin/permission-sets");
        }
    }
}