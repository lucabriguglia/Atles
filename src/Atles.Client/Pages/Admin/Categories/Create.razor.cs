using System.Threading.Tasks;
using Atles.Client.Components.Admin;
using Atles.Models.Admin;

namespace Atles.Client.Pages.Admin.Categories
{
    public abstract class CreatePage : AdminPageBase
    {
        protected CategoryFormModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<CategoryFormModel>("api/admin/categories/create");
        }

        protected async Task SaveAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/categories/save", Model.Category);
            NavigationManager.NavigateTo("/admin/categories");
        }
    }
}