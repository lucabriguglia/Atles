using System.Threading.Tasks;
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

        public bool UserIsAuthenticated { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var claimsPrincipal = await GetClaimsPrincipal();
            UserIsAuthenticated = claimsPrincipal.Identity.IsAuthenticated;
        }
    }
}