using System;
using System.Threading.Tasks;
using Atles.Client.Shared;
using Atles.Models;
using Atles.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atles.Client.Components.Themes
{
    public abstract class ForumComponent : ThemeComponentBase
    {
        [Parameter] public string Slug { get; set; }

        protected ForumPageModel Model { get; set; }
        protected int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        protected string Search { get; set; }
        protected bool DisplayPage { get; set; }

        protected PagerComponent Pager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Model = await ApiService.GetFromJsonAsync<ForumPageModel>($"api/public/forums/{Slug}?page=1");
                TotalPages = Model.Topics.TotalPages;
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
            Model.Topics = null;
            await LoadTopicsAsync();
            Pager.ReInitialize(TotalPages);
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = string.Empty;
                CurrentPage = 1;
                Model.Topics = null;
                await LoadTopicsAsync();
                Pager.ReInitialize(TotalPages);
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
            await JsRuntime.InvokeVoidAsync("atlas.interop.scrollToTarget", "topics");
            CurrentPage = page;
            await LoadTopicsAsync();
        }

        private async Task LoadTopicsAsync()
        {
            Model.Topics = await ApiService.GetFromJsonAsync<PaginatedData<ForumPageModel.TopicModel>>($"api/public/forums/{Model.Forum.Id}/topics?page={CurrentPage}&search={Search}");
            TotalPages = Model.Topics.TotalPages;
        }
    }
}
