using System.Threading.Tasks;
using Docs.Models;
using Atles.Client.Components.Pages;

namespace Atles.Client.Pages
{
    public abstract class DocsPage : PageBase
    {
        protected DocumentationModel Model { get; set; }

        //public TargetModel SelectedTarget { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<DocumentationModel>("api/public/documentation");
            //SelectedTarget = Model.Contexts[0].Targets[0];
        }

        //protected void SelectTarget(string contextName, string targetName)
        //{
        //    var selectedContext = Model.Contexts.FirstOrDefault(x => x.Name == contextName);
        //    SelectedTarget = selectedContext?.Targets.FirstOrDefault(x => x.Name == targetName);
        //}
    }
}