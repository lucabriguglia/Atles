using System.Net;
using Atles.Client.Components.Admin;
using Atles.Models.Admin.Categories;

namespace Atles.Client.Pages.Admin.Categories;

public abstract class CreatePage : AdminPageBase
{
    protected CategoryFormModel Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = await ApiService.GetFromJsonAsync<CategoryFormModel>("api/admin/categories/create");
    }

    protected async Task SaveAsync()
    {
        var response = await ApiService.PostAsJsonAsync("api/admin/categories/save", Model.Category);

        if (response.IsSuccessStatusCode)
        {
            NavigationManager.NavigateTo("admin/categories");
        }
        else
        {
            // TODO: Display error message
        }
    }

    protected void Cancel()
    {
        NavigationManager.NavigateTo("/admin/categories");
    }
}