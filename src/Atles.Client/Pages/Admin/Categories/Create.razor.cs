using System.Net;
using System.Threading.Tasks;
using Atles.Client.Components.Admin;
using Atles.Models.Admin;
using Atles.Models.Admin.Categories;

namespace Atles.Client.Pages.Admin.Categories;

public abstract class CreatePage : AdminPageBase
{
    protected CreateCategoryFormModel Model { get; set; }
    protected string ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = await ApiService.GetFromJsonAsync<CreateCategoryFormModel>("api/admin/categories/create");
    }

    protected async Task SaveAsync()
    {
        var response = await ApiService.PostAsJsonAsync("api/admin/categories/save", Model.Category);
        if (response.StatusCode is HttpStatusCode.OK)
        {
            NavigationManager.NavigateTo("admin/categories");
        }
        else
        {
            ErrorMessage = response.StatusCode.ToString();
        }
    }

    protected void Cancel()
    {
        NavigationManager.NavigateTo("/admin/categories");
    }
}