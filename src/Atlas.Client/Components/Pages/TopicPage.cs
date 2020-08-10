using System;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Pages
{
    public abstract class TopicPage : PageBase
    {
        [Parameter] public Guid ForumId { get; set; }
        [Parameter] public Guid TopicId { get; set; }
    }
}