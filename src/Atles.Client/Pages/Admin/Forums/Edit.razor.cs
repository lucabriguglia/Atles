using System.Net;
using Atles.Client.Components.Admin;
using Atles.Models.Admin.Forums;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.Forums;

public abstract class EditPage : AdminPageBase
{
    [Parameter] public Guid Id { get; set; }

    protected ForumFormModel Model { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Model = await ApiService.GetFromJsonAsync<ForumFormModel>($"api/admin/forums/edit/{Id}");
    }

    protected async Task UpdateAsync()
    {
        var response = await ApiService.PostAsJsonAsync("api/admin/forums/update", Model.Forum);
        
        if (response.IsSuccessStatusCode)
        {
            NavigationManager.NavigateTo($"/admin/forums/{Model.Forum.CategoryId}");
        }
        else
        {
            // TODO: Display error message
        }
    }

    protected void Cancel()
    {
        NavigationManager.NavigateTo("/admin/forums");
    }
}
