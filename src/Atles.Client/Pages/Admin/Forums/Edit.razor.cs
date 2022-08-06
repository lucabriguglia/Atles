using Atles.Client.Components.Admin;
using Atles.Models.Admin.Forums;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.Forums;

public abstract class EditPage : AdminPageBase
{
    [Parameter] public Guid Id { get; set; }

    protected CreateForumFormModel Model { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Model = await ApiService.GetFromJsonAsync<CreateForumFormModel>($"api/admin/forums/edit/{Id}");
    }

    protected async Task UpdateAsync()
    {
        await ApiService.PostAsJsonAsync($"api/admin/forums/update", Model.Forum);
        NavigationManager.NavigateTo($"/admin/forums/{Model.Forum.CategoryId}");
    }

    protected void Cancel()
    {
        NavigationManager.NavigateTo("/admin/forums");
    }
}
