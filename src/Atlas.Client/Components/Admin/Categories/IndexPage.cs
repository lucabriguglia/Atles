using System;
using System.Threading.Tasks;
using Atlas.Models.Admin.Categories;
using Microsoft.AspNetCore.Components.Web;

namespace Atlas.Client.Components.Admin.Categories
{
    public abstract class IndexPage : AdminPageBase
    {
        protected IndexPageModel Model { get; set; }
        protected Guid DeleteId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<IndexPageModel>("api/admin/categories/list");
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