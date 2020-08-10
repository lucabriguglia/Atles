using System.Threading.Tasks;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atlas.Client.Components.Themes
{
    public class SearchComponentBase : ThemeComponentBase
    {
        protected SearchPageModel Model { get; set; }

        protected string SearchTerm { get; set; }
        protected int CurrentPage { get; set; } = 1;

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<SearchPageModel>("api/public/search?page=1");
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
            Model.Posts = null;
            StateHasChanged();
            await LoadDataAsync();
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                SearchTerm = string.Empty;
                Model.Posts = null;
                StateHasChanged();
                await LoadDataAsync();
            }
        }

        protected async Task ChangePageAsync(int page)
        {
            await JsRuntime.InvokeVoidAsync("scrollToTarget", "posts");
            CurrentPage = page;
            Model.Posts = null;
            StateHasChanged();
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            Model = await ApiService.GetFromJsonAsync<SearchPageModel>($"api/public/search?page={CurrentPage}&search={SearchTerm}");
        }
    }
}