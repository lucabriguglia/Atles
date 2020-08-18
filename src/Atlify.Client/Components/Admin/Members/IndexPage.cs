using System;
using System.Threading.Tasks;
using Atlify.Models.Admin.Members;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atlify.Client.Components.Admin.Members
{
    public abstract class IndexPage : AdminPageBase
    {
        protected IndexPageModel Model { get; set; }
        protected Guid SuspendId { get; set; }
        protected Guid DeleteId { get; set; }
        protected int CurrentPage { get; set; } = 1;
        protected string Search { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<IndexPageModel>($"api/admin/members/index-model");
        }

        private async Task LoadMembersAsync()
        {
            Model = await ApiService.GetFromJsonAsync<IndexPageModel>($"api/admin/members/index-model?page={CurrentPage}&search={Search}");
        }

        protected async Task ChangePageAsync(int page)
        {
            CurrentPage = page;
            Model.Members = null;
            await JsRuntime.InvokeVoidAsync("scrollToTarget", "members");
            await LoadMembersAsync();
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
            CurrentPage = 1;
            Model.Members = null;
            StateHasChanged();
            await LoadMembersAsync();
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = string.Empty;
                CurrentPage = 1;
                Model = null;
                StateHasChanged();
                await LoadMembersAsync();
            }
        }

        protected void SetSuspendId(Guid id)
        {
            SuspendId = id;
        }

        protected async Task SuspendAsync(MouseEventArgs e)
        {
            await ApiService.PostAsJsonAsync("api/admin/members/suspend", SuspendId);
            Model.Members = null;
            StateHasChanged();
            await LoadMembersAsync();
        }

        protected async Task ReinstateAsync(Guid id)
        {
            Model.Members = null;
            await ApiService.PostAsJsonAsync("api/admin/members/reinstate", id);
            StateHasChanged();
            await LoadMembersAsync();
        }

        protected void SetDeleteId(Guid id)
        {
            DeleteId = id;
        }

        protected async Task DeleteAsync(MouseEventArgs e)
        {
            Model.Members = null;
            await ApiService.DeleteAsync($"api/admin/members/delete/{DeleteId}");
            StateHasChanged();
            await OnInitializedAsync();
        }
    }
}