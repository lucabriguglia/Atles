using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Themes
{
    public abstract class ThemeLayoutComponentBase : LayoutComponentBase
    {
        [CascadingParameter] protected CurrentSiteModel Site { get; set; }
    }
}