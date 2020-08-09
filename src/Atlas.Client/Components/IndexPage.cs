using System.Threading.Tasks;
using Atlas.Models.Public;

namespace Atlas.Client.Components
{
    public abstract class IndexPage : PageBase
    {
        protected IndexPageModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Model = await ApiService.GetFromJsonAsync<IndexPageModel>("api/public/index-model");
        }
    }
}