using System.Net;
using Atles.Client.Components.Admin;
using Atles.Models.Admin.Forums;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.Forums;

public abstract class CreatePage : AdminPageBase
{
    [Parameter] public Guid? CategoryId { get; set; }

    protected CreateForumFormModel Model { get; set; }
    protected string ErrorMessage { get; set; }
    protected override async Task OnInitializedAsync()
    {
        var requestUri = CategoryId == null
            ? "api/admin/forums/create"
            : $"api/admin/forums/create/{CategoryId}";

        Model = await ApiService.GetFromJsonAsync<CreateForumFormModel>(requestUri);
    }

    protected async Task SaveAsync()
    {
        var response = await ApiService.PostAsJsonAsync("api/admin/forums/save", Model.Forum);

        if (response.StatusCode is HttpStatusCode.OK)
        {
            NavigationManager.NavigateTo($"/admin/forums/{Model.Forum.CategoryId}");
        }
        else
        {
            ErrorMessage = response.StatusCode.ToString();
        }
    }

    protected void Cancel()
    {
        NavigationManager.NavigateTo($"/admin/forums/{Model.Forum.CategoryId}");
    }
}
