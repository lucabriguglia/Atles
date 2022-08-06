using System.Net;
using Atles.Client.Components.Admin;
using Atles.Models.Admin.Forums;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.Forums;

public abstract class CreatePage : AdminPageBase
{
    [Parameter] public Guid? CategoryId { get; set; }

    protected ForumFormModel Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var requestUri = CategoryId == null
            ? "api/admin/forums/create"
            : $"api/admin/forums/create/{CategoryId}";

        Model = await ApiService.GetFromJsonAsync<ForumFormModel>(requestUri);
    }

    protected async Task SaveAsync()
    {
        var response = await ApiService.PostAsJsonAsync("api/admin/forums/save", Model.Forum);

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
        NavigationManager.NavigateTo($"/admin/forums/{Model.Forum.CategoryId}");
    }
}
