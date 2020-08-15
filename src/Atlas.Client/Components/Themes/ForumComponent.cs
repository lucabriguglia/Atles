using Atlas.Models;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using Atlas.Models.Public.Forums;

namespace Atlas.Client.Components.Themes
{
    public abstract class ForumComponent : ThemeComponentBase
    {
        [Parameter] public Guid Id { get; set; }

        protected ForumPageModel Model { get; set; }
        protected int CurrentPage { get; set; } = 1;
        protected string Search { get; set; }
        protected bool DisplayPage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Model = await ApiService.GetFromJsonAsync<ForumPageModel>($"api/public/forums/{Id}?page=1");
                DisplayPage = true;
            }
            catch (Exception)
            {
                Model = new ForumPageModel();
                DisplayPage = false;
            }
        }

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
            Model.Topics = await ApiService.GetFromJsonAsync<PaginatedData<ForumPageModel.TopicModel>>($"api/public/forums/{Id}/topics?page={CurrentPage}&search={Search}");
        }
    }
}
