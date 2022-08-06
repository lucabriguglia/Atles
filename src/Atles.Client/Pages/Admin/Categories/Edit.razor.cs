using Atles.Client.Components.Admin;
using Atles.Models.Admin.Categories;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.Categories;

public abstract class EditPage : AdminPageBase
{
    [Parameter] public Guid Id { get; set; }

    protected CategoryFormModel Model { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Model = await ApiService.GetFromJsonAsync<CategoryFormModel>($"api/admin/categories/edit/{Id}");
    }

    protected async Task UpdateAsync()
    {
        var response = await ApiService.PostAsJsonAsync("api/admin/categories/update", Model.Category);

        if (response.IsSuccessStatusCode)
        {
            NavigationManager.NavigateTo("/admin/categories");
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
