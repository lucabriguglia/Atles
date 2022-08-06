using Atles.Client.Components.Admin;
using Atles.Models.Admin.PermissionSets;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.PermissionSets;

public abstract class EditPage : AdminPageBase
{
    [Parameter] public Guid Id { get; set; }

    protected PermissionSetFormModel Model { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Model = await ApiService.GetFromJsonAsync<PermissionSetFormModel>($"api/admin/permission-sets/edit/{Id}");
    }

    protected async Task UpdateAsync()
    {
        var response = await ApiService.PostAsJsonAsync("api/admin/permission-sets/update", Model.PermissionSet);

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
