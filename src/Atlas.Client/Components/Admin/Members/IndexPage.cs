using System;
using System.Threading.Tasks;
using Atlas.Client.Components.Shared;
using Atlas.Models.Admin.Members;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atlas.Client.Components.Admin.Members
{
    public abstract class IndexPage : AdminPageBase
    {
        protected IndexPageModel Model { get; set; }
        protected Guid SuspendId { get; set; }
        protected Guid DeleteId { get; set; }
        protected int CurrentPage { get; set; } = 1;
        protected int TotalPages { get; set; } = 1;
        protected string Search { get; set; }
        protected string Status { get; set; }
        protected string SortByField { get; set; }
        protected string SortByDirection { get; set; }
        protected PagerComponent Pager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<IndexPageModel>("api/admin/members/index-model");
            TotalPages = Model.Members.TotalPages;
        }

        private async Task LoadMembersAsync()
        {
            var model = await ApiService.GetFromJsonAsync<IndexPageModel>($"api/admin/members/index-model?page={CurrentPage}&search={Search}&status={Status}&sortByField={SortByField}&sortByDirection={SortByDirection}");
            Model.Members = model.Members;
            TotalPages = Model.Members.TotalPages;
        }

        protected async Task ChangePageAsync(int page)
        {
            CurrentPage = page;
            Model.Members = null;
            await JsRuntime.InvokeVoidAsync("atlas.interop.scrollToTarget", "members");
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
            await LoadMembersAsync();
            Pager.ReInitialize(TotalPages);
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = string.Empty;
                CurrentPage = 1;
                Model.Members = null;
                await LoadMembersAsync();
                Pager.ReInitialize(TotalPages);
            }
        }

        protected async Task StatusChangedAsync(ChangeEventArgs args)
        {
            Status = args.Value.ToString();
            CurrentPage = 1;
            Model.Members = null;
            await LoadMembersAsync();
            Pager.ReInitialize(TotalPages);
        }

        protected async Task SortByChangedAsync(ChangeEventArgs args)
        {
            var selected = args.Value.ToString().Split('-');
            SortByField = selected[0];
            SortByDirection = selected[1];
            CurrentPage = 1;
            Model.Members = null;
            await LoadMembersAsync();
        }

        protected void SetSuspendId(Guid id)
        {
            SuspendId = id;
        }

        protected async Task SuspendAsync(MouseEventArgs e)
        {
            await ApiService.PostAsJsonAsync("api/admin/members/suspend", SuspendId);
            Model.Members = null;
            await LoadMembersAsync();
        }

        protected async Task ReinstateAsync(Guid id)
        {
            Model.Members = null;
            await ApiService.PostAsJsonAsync("api/admin/members/reinstate", id);
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
            await OnInitializedAsync();
        }
    }
}