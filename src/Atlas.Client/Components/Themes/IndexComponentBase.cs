using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Themes
{
    public class IndexComponentBase : ThemeComponentBase
    {
        [Parameter]  public IndexPageModel Model { get; set; }
    }
}