using System;
using System.Threading.Tasks;
using Atlify.Models.Admin.Members;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atlify.Client.Components.Admin.Members
{
    public abstract class ActivityPage : AdminPageBase
    {
        [Parameter] public Guid Id { get; set; }

        protected ActivityPageModel Model { get; set; }

        public int CurrentPage { get; set; } = 1;
        protected string Search { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<ActivityPageModel>($"api/admin/members/activity/{Id}");
        }

        private async Task LoadEventsAsync()
        {
            var model = await ApiService.GetFromJsonAsync<ActivityPageModel>($"api/admin/members/activity/{Id}?page={CurrentPage}&search={Search}");
            Model.Events = model.Events;
        }

        protected async Task ChangePageAsync(int page)
        {
            CurrentPage = page;
            Model.Events = null;
            await JsRuntime.InvokeVoidAsync("scrollToTarget", "events");
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
            StateHasChanged();
            await LoadEventsAsync();
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = string.Empty;
                CurrentPage = 1;
                Model.Events = null;
                StateHasChanged();
                await LoadEventsAsync();
            }
        }
    }
}