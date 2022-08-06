using Atles.Client.Components.Admin;
using Atles.Models.Admin.PermissionSets;

namespace Atles.Client.Pages.Admin.PermissionSets;

public abstract class CreatePage : AdminPageBase
{
    protected PermissionSetFormModel Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = await ApiService.GetFromJsonAsync<PermissionSetFormModel>("api/admin/permission-sets/create");
    }

    protected async Task SaveAsync()
    {
        var response = await ApiService.PostAsJsonAsync("api/admin/permission-sets/save", Model.PermissionSet);
            
        if (response.IsSuccessStatusCode)
        {
            NavigationManager.NavigateTo("/admin/permission-sets");
        }
        else
        {
            // TODO: Display error message
        }
    }

    protected void Cancel()
    {
        NavigationManager.NavigateTo("/admin/permission-sets");
    }
}
