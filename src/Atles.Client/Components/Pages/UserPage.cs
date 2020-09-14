using System;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Components.Pages
{
    public abstract class UserPage : PageBase
    {
        [Parameter] public Guid Id { get; set; }
    }
}