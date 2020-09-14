using Microsoft.AspNetCore.Components;

namespace Atles.Client.Components.Pages
{
    public abstract class ForumPage : PageBase
    {
        [Parameter] public string Slug { get; set; }
    }
}