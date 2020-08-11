using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Themes
{
    public abstract class PublicLayoutComponent : LayoutComponentBase
    {
        [CascadingParameter] protected CurrentMemberModel Member { get; set; }
        [CascadingParameter] protected CurrentSiteModel Site { get; set; }
    }
}