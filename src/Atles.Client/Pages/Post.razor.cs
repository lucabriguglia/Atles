using System;
using Microsoft.AspNetCore.Components;
using Atles.Client.Components.Pages;

namespace Atles.Client.Pages
{
    public abstract class PostPage : PageBase
    {
        [Parameter] public Guid ForumId { get; set; }
        [Parameter] public Guid? TopicId { get; set; }
    }
}