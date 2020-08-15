using System.Threading.Tasks;
using Atlas.Models.Public;
using Atlas.Models.Public.Index;

namespace Atlas.Client.Components.Themes
{
    public abstract class IndexComponent : ThemeComponentBase
    {
        protected IndexPageModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Model = await ApiService.GetFromJsonAsync<IndexPageModel>("api/public/index-model");
        }
    }
}