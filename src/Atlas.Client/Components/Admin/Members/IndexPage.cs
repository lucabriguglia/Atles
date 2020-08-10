using System;
using System.Threading.Tasks;
using Atlas.Models.Admin.Members;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atlas.Client.Components.Admin.Members
{
    public abstract class IndexPage : AdminPageBase
    {
        protected IndexPageModel Model { get; set; }
        protected Guid DeleteId { get; set; }
        protected string Search { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync(1);
        }

        private async Task LoadDataAsync(int page)
        {
            Model = await ApiService.GetFromJsonAsync<IndexPageModel>($"api/admin/members/index-model?page={page}&search={Search}");
        }

        protected async Task ChangePageAsync(int page)
        {
            await JsRuntime.InvokeVoidAsync("scrollToTarget", "members");
            await LoadDataAsync(page);
        }

        protected async Task MyKeyUpAsync(KeyboardEventArgs key)
        {
            if (key.Code == "Enter")
            {
                await SearchAsync();
            }
        }

        protected async Task SearchAsync()
        {
            Model = null;
            StateHasChanged();
            await OnInitializedAsync();
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = string.Empty;
                Model = null;
                StateHasChanged();
                await OnInitializedAsync();
            }
        }

        protected void SetDeleteId(Guid id)
        {
            DeleteId = id;
        }

        protected async Task DeleteAsync(MouseEventArgs e)
        {
            await ApiService.DeleteAsync($"api/admin/members/delete/{DeleteId}");
            Model = null;
            StateHasChanged();
            await OnInitializedAsync();
        }
    }
}