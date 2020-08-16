using System;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Pages
{
    public abstract class ForumPage : PageBase
    {
        [Parameter] public Guid? Id { get; set; }
        [Parameter] public string Slug { get; set; }
    }
}