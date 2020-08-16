using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Pages
{
    public abstract class TopicPage : PageBase
    {
        [Parameter] public string ForumSlug { get; set; }
        [Parameter] public string TopicSlug { get; set; }
    }
}