using System;
using System.Threading.Tasks;
using Atles.Client.Components.Admin;
using Atles.Models.Admin.Categories;
using Microsoft.AspNetCore.Components.Web;

namespace Atles.Client.Pages.Admin.Categories
{
    public abstract class IndexPage : AdminPageBase
    {
        protected CategoriesPageModel Model { get; set; }
        protected Guid DeleteId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<CategoriesPageModel>("api/admin/categories/list");
        }

        protected async Task MoveUpAsync(Guid id)
        {
            await ApiService.PostAsJsonAsync("api/admin/categories/move-up", id);
            await OnInitializedAsync();
        }

        protected async Task MoveDownAsync(Guid id)
        {
            await ApiService.PostAsJsonAsync("api/admin/categories/move-down", id);
            await OnInitializedAsync();
        }

        protected void SetDeleteId(Guid id)
        {
            DeleteId = id;
        }

        protected async Task DeleteAsync(MouseEventArgs e)
        {
            await ApiService.DeleteAsync($"api/admin/categories/delete/{DeleteId}");
            await OnInitializedAsync();
        }
    }
}