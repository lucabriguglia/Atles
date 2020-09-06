using System.Linq;
using System.Threading.Tasks;
using Docs.Models;

namespace Atlas.Client.Components.Admin
{
    public abstract class DocsPage : AdminPageBase
    {
        protected DocumentationModel Model { get; set; }

        public TargetModel SelectedTarget { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<DocumentationModel>("api/admin/documentation");
            SelectedTarget = Model.Contexts[0].Targets[0];
        }

        protected void SelectTarget(string contextName, string targetName)
        {
            var selectedContext = Model.Contexts.FirstOrDefault(x => x.Name == contextName);
            SelectedTarget = selectedContext?.Targets.FirstOrDefault(x => x.Name == targetName);
        }
    }
}