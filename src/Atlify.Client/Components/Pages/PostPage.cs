using System;
using Microsoft.AspNetCore.Components;

namespace Atlify.Client.Components.Pages
{
    public abstract class PostPage : PageBase
    {
        [Parameter] public Guid ForumId { get; set; }
        [Parameter] public Guid? TopicId { get; set; }
    }
}