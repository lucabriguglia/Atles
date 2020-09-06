using System.Threading.Tasks;
using Docs.Models;

namespace Atlas.Client.Components.Admin
{
    public abstract class DocsPage : AdminPageBase
    {
        protected DocumentationModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<DocumentationModel>("api/admin/documentation");
        }
    }
}