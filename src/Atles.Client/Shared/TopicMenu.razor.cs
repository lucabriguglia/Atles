using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Atles.Client.Components.Shared;

namespace Atles.Client.Shared
{
    public abstract class TopicMenuComponent : SharedComponentBase
    {
        [Parameter] public Guid TopicId { get; set; }
        [Parameter] public Guid ForumId { get; set; }

        [Parameter] public bool CanEdit { get; set; }
        [Parameter] public bool CanModerate { get; set; }
        [Parameter] public bool CanDelete { get; set; }

        [Parameter] public bool Pinned { get; set; }
        [Parameter] public bool Locked { get; set; }

        [Parameter] public bool Subscribed { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> EditCallback { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> PinCallback { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> LockCallback { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> AddSubscriptionCallback { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> RemoveSubscriptionCallback { get; set; }

        protected string PinButtonText => Pinned
            ? Loc["Unpin"]
            : Loc["Pin"];

        protected string LockButtonText => Locked
            ? Loc["Unlock"]
            : Loc["Lock"];

        protected bool Display { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var claimsPrincipal = await GetClaimsPrincipal();
            Display = claimsPrincipal.Identity.IsAuthenticated;
        }
    }
}