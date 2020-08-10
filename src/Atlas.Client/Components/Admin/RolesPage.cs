using System.Threading.Tasks;
using Atlas.Models.Admin.Roles;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atlas.Client.Components.Admin
{
    public abstract class RolesPage : AdminPageBase
    {
        protected IndexPageModel Model { get; set; }

        protected string EditTitle => IsEdit ? Loc["Edit Role"] : Loc["Create New Role"];
        protected string EditButton => IsEdit ? Loc["Update"] : Loc["Save"];

        protected bool IsEdit { get; set; }
        protected string DeleteId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IsEdit = false;
            Model = await ApiService.GetFromJsonAsync<IndexPageModel>("api/admin/roles/list");
        }

        protected async Task NewAsync()
        {
            await JsRuntime.InvokeVoidAsync("scrollToTarget", "form");
        }

        protected async Task EditAsync(string id, string name)
        {
            IsEdit = true;
            Model.EditRole.Id = id;
            Model.EditRole.Name = name;
            await JsRuntime.InvokeVoidAsync("scrollToTarget", "form");
        }

        protected async Task SaveAsync()
        {
            var action = IsEdit ? "update" : "create";
            await ApiService.PostAsJsonAsync($"api/admin/roles/{action}", Model.EditRole);
            await OnInitializedAsync();
        }

        protected void Cancel()
        {
            IsEdit = false;
            Model.EditRole.Id = null;
            Model.EditRole.Name = null;
        }

        protected async Task DeleteAsync(MouseEventArgs e)
        {
            await ApiService.DeleteAsync($"api/admin/roles/delete/{DeleteId}");
            await OnInitializedAsync();
        }

        protected void SetDeleteId(string id)
        {
            DeleteId = id;
        }
    }
}