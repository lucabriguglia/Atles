using System.Threading.Tasks;
using Atlas.Models.Public;

namespace Atlas.Client.Components.Themes
{
    public class IndexComponentBase : ThemeComponentBase
    {
        protected IndexPageModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Model = await ApiService.GetFromJsonAsync<IndexPageModel>("api/public/index-model");
        }
    }
}