using System.Threading.Tasks;
using Atles.Client.Components.Admin;
using Atles.Reporting.Models.Admin.Categories;

namespace Atles.Client.Pages.Admin.Categories
{
    public abstract class CreatePage : AdminPageBase
    {
        protected FormComponentModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<FormComponentModel>("api/admin/categories/create");
        }

        protected async Task SaveAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/categories/save", Model.Category);
            NavigationManager.NavigateTo("/admin/categories");
        }
    }
}