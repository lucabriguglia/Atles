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
    public abstract class ActivityPage : AdminPageBase
    {
        [Parameter] public Guid Id { get; set; }

        protected ActivityPageModel Model { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        protected string Search { get; set; }

        protected PagerComponent Pager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<ActivityPageModel>($"api/admin/users/activity/{Id}");
            TotalPages = Model.Events.TotalPages;
        }

        private async Task LoadEventsAsync()
        {
            var model = await ApiService.GetFromJsonAsync<ActivityPageModel>($"api/admin/users/activity/{Id}?page={CurrentPage}&search={Search}");
            Model.Events = model.Events;
            TotalPages = Model.Events.TotalPages;
        }

        protected async Task ChangePageAsync(int page)
        {
            CurrentPage = page;
            Model.Events = null;
            await JsRuntime.InvokeVoidAsync("atlas.interop.scrollToTarget", "events");
            await LoadEventsAsync();
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
            Model.Events = null;
            await LoadEventsAsync();
            Pager.ReInitialize(TotalPages);
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = string.Empty;
                CurrentPage = 1;
                Model.Events = null;
                await LoadEventsAsync();
                Pager.ReInitialize(TotalPages);
            }
        }
    }
}