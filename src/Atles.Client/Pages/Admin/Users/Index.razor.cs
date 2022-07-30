using System;
using System.Threading.Tasks;
using Atles.Client.Components.Admin;
using Atles.Client.Shared;
using Atles.Models.Admin.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atles.Client.Pages.Admin.Users
{
    public abstract class IndexPage : AdminPageBase
    {
        protected IndexPageModel Model { get; set; }
        protected Guid SuspendId { get; set; }
        protected Guid DeleteId { get; set; }
        protected string DeleteIdentityUserId { get; set; }
        protected int CurrentPage { get; set; } = 1;
        protected int TotalPages { get; set; } = 1;
        protected string Search { get; set; }
        protected string Status { get; set; }
        protected string SortByField { get; set; }
        protected string SortByDirection { get; set; }
        protected PagerComponent Pager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<IndexPageModel>("api/admin/users/index-model");
            TotalPages = Model.Users.TotalPages;
        }

        private async Task LoadUsersAsync()
        {
            var model = await ApiService.GetFromJsonAsync<IndexPageModel>($"api/admin/users/index-model?page={CurrentPage}&search={Search}&status={Status}&sortByField={SortByField}&sortByDirection={SortByDirection}");
            Model.Users = model.Users;
            TotalPages = Model.Users.TotalPages;
        }

        protected async Task ChangePageAsync(int page)
        {
            CurrentPage = page;
            Model.Users = null;
            await JsRuntime.InvokeVoidAsync("atlas.interop.scrollToTarget", "users");
            await LoadUsersAsync();
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
            Model.Users = null;
            await LoadUsersAsync();
            Pager.ReInitialize(TotalPages);
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = string.Empty;
                CurrentPage = 1;
                Model.Users = null;
                await LoadUsersAsync();
                Pager.ReInitialize(TotalPages);
            }
        }

        protected async Task StatusChangedAsync(ChangeEventArgs args)
        {
            Status = args.Value.ToString();
            CurrentPage = 1;
            Model.Users = null;
            await LoadUsersAsync();
            Pager.ReInitialize(TotalPages);
        }

        protected async Task SortByChangedAsync(ChangeEventArgs args)
        {
            var selected = args.Value.ToString().Split('-');
            SortByField = selected[0];
            SortByDirection = selected[1];
            CurrentPage = 1;
            Model.Users = null;
            await LoadUsersAsync();
        }

        protected void SetSuspendId(Guid id)
        {
            SuspendId = id;
        }

        protected async Task SuspendAsync(MouseEventArgs e)
        {
            await ApiService.PostAsJsonAsync("api/admin/users/suspend", SuspendId);
            Model.Users = null;
            await LoadUsersAsync();
        }

        protected async Task ReinstateAsync(Guid id)
        {
            Model.Users = null;
            await ApiService.PostAsJsonAsync("api/admin/users/reinstate", id);
            await LoadUsersAsync();
        }

        protected void SetDeleteIds(Guid id, string identityUserId)
        {
            DeleteId = id;
            DeleteIdentityUserId = identityUserId;
        }

        protected async Task DeleteAsync(MouseEventArgs e)
        {
            Model.Users = null;
            await ApiService.DeleteAsync($"api/admin/users/delete/{DeleteId}/{DeleteIdentityUserId}");
            await OnInitializedAsync();
        }
    }
}