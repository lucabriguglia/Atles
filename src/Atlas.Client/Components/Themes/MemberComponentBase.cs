using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Themes
{
    public class MemberComponentBase : ThemeComponentBase
    {
        [Parameter] public MemberPageModel Model { get; set; }
    }
}