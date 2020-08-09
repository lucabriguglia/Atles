using Atlas.Models;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Atlas.Client.Components.Themes
{
    public class ForumComponentBase : ThemeComponentBase
    {
        [Parameter] public ForumPageModel Model { get; set; }
        [Parameter]  public Guid Id { get; set; }

        public int CurrentPage { get; set; } = 1;
        public string Search { get; set; }

        protected async Task SearchAsync()
        {
            CurrentPage = 1;
            await LoadTopicsAsync();
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = string.Empty;
                CurrentPage = 1;
                await LoadTopicsAsync();
            }
        }

        protected async Task MyKeyUpAsync(KeyboardEventArgs key)
        {
            if (key.Code == "Enter")
            {
                await SearchAsync();
            }
        }

        protected async Task ChangePageAsync(int page)
        {
            await JsRuntime.InvokeVoidAsync("scrollToTarget", "topics");
            CurrentPage = page;
            await LoadTopicsAsync();
        }

        private async Task LoadTopicsAsync()
        {
            Model.Topics = null;
            Model.Topics = await ApiService.GetFromJsonAsync<PaginatedData<ForumPageModel.TopicModel>>($"api/public/forums/{Id}/topics?page={CurrentPage}&search={Search}");
        }
    }
}