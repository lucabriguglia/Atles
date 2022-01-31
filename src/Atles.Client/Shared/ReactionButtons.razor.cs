using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Atles.Client.Components.Shared;

namespace Atles.Client.Shared
{
    public abstract class ReactionButtonsComponent : SharedComponentBase
    {
        [Parameter] public bool CanReact { get; set; }
        [Parameter] public bool Reacted { get; set; }
        [Parameter] public int ReactionsCount { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> AddReactionCallback { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> RemoveReactionCallback { get; set; }

        protected override void OnInitialized()
        {
        }
    }
}