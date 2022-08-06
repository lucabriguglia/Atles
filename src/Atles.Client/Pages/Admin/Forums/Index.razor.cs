using System;
using System.Threading.Tasks;
using Atles.Client.Components.Admin;
using Atles.Models.Admin.Forums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Atles.Client.Pages.Admin.Forums
{
    public abstract class IndexPage : AdminPageBase
    {
        [Parameter] public Guid? CategoryId { get; set; }

        protected ForumsPageModel Model { get; set; }
        protected Guid DeleteId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var requestUri = CategoryId == null
                ? "api/admin/forums/index-model"
                : $"api/admin/forums/index-model/{CategoryId}";

            Model = await ApiService.GetFromJsonAsync<ForumsPageModel>(requestUri);
        }

        protected async Task CategoryChangedAsync(ChangeEventArgs args)
        {
            CategoryId = new Guid(args.Value.ToString());
            Model = await ApiService.GetFromJsonAsync<ForumsPageModel>($"api/admin/forums/index-model/{CategoryId}");
            StateHasChanged();
        }

        protected async Task MoveUpAsync(Guid id)
        {
            await ApiService.PostAsJsonAsync("api/admin/forums/move-up", id);
            await OnInitializedAsync();
        }

        protected async Task MoveDownAsync(Guid id)
        {
            await ApiService.PostAsJsonAsync("api/admin/forums/move-down", id);
            await OnInitializedAsync();
        }

        protected void SetDeleteId(Guid id)
        {
            DeleteId = id;
        }

        protected async Task DeleteAsync(MouseEventArgs e)
        {
            await ApiService.DeleteAsync($"api/admin/forums/delete/{DeleteId}");
            await OnInitializedAsync();
        }
    }
}