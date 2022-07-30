using System.Threading.Tasks;
using Atles.Client.Shared;
using Atles.Models.Public;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atles.Client.Components.Themes
{
    public abstract class SearchComponent : ThemeComponentBase
    {
        protected SearchPageModel Model { get; set; }

        protected string SearchTerm { get; set; }
        protected int CurrentPage { get; set; } = 1;
        protected int TotalPages { get; set; }

        protected PagerComponent Pager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<SearchPageModel>("api/public/search?page=1");
            TotalPages = Model.Posts.TotalPages;
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
            Model.Posts = null;
            await LoadDataAsync();
            Pager.ReInitialize(TotalPages);
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                SearchTerm = string.Empty;
                CurrentPage = 1;
                Model.Posts = null;
                await LoadDataAsync();
                Pager.ReInitialize(TotalPages);
            }
        }

        protected async Task ChangePageAsync(int page)
        {
            await JsRuntime.InvokeVoidAsync("atlas.interop.scrollToTarget", "posts");
            CurrentPage = page;
            Model.Posts = null;
            StateHasChanged();
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            Model = await ApiService.GetFromJsonAsync<SearchPageModel>($"api/public/search?page={CurrentPage}&search={SearchTerm}");
            TotalPages = Model.Posts.TotalPages;
        }
    }
}