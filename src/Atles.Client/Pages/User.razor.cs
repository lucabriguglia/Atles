using System;
using Microsoft.AspNetCore.Components;
using Atles.Client.Components.Pages;

namespace Atles.Client.Pages
{
    public abstract class UserPage : PageBase
    {
        [Parameter] public Guid Id { get; set; }
    }
}