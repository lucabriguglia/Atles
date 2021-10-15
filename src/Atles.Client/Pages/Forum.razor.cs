using Microsoft.AspNetCore.Components;
using Atles.Client.Components.Pages;

namespace Atles.Client.Pages
{
    public abstract class ForumPage : PageBase
    {
        [Parameter] public string Slug { get; set; }
    }
}