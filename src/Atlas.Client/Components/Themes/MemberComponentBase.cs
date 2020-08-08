using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Themes
{
    public class MemberComponentBase : ComponentBase
    {
        [Parameter]
        public MemberPageModel Model { get; set; }
    }
}