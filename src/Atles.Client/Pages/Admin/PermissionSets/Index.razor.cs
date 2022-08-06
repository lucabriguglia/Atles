using Atles.Client.Components.Admin;
using Atles.Models.Admin.PermissionSets;
using Microsoft.AspNetCore.Components.Web;

namespace Atles.Client.Pages.Admin.PermissionSets
{
    public abstract class IndexPage : AdminPageBase
    {
        protected PermissionSetsPageModel Model { get; set; }
        protected Guid DeleteId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<PermissionSetsPageModel>("api/admin/permission-sets/list");
        }

        protected void SetDeleteId(Guid id)
        {
            DeleteId = id;
        }

        protected async Task DeleteAsync(MouseEventArgs e)
        {
            await ApiService.DeleteAsync($"api/admin/permission-sets/delete/{DeleteId}");
            await OnInitializedAsync();
        }
    }
}