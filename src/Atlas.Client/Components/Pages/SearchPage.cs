using System.Threading.Tasks;
using Atlas.Models.Public;

namespace Atlas.Client.Components.Pages
{
    public abstract class SearchPage : PageBase
    {
        public SearchPageModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<SearchPageModel>("api/public/search?page=1");
        }
    }
}