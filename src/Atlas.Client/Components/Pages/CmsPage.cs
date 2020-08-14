using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Pages
{
    public abstract class CmsPage : PageBase
    {
        [Parameter] public string Page { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}