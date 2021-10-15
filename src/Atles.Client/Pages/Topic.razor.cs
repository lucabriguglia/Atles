using Microsoft.AspNetCore.Components;
using Atles.Client.Components.Pages;

namespace Atles.Client.Pages
{
    public abstract class TopicPage : PageBase
    {
        [Parameter] public string ForumSlug { get; set; }
        [Parameter] public string TopicSlug { get; set; }
    }
}