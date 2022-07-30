using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Atles.Client.Components.Shared;
using Atles.Client.Models;
using Atles.Domain;

namespace Atles.Client.Shared
{
    public abstract class ReactionButtonsComponent : SharedComponentBase
    {
        [Parameter] public Guid PostId { get; set; }
        [Parameter] public bool CanReact { get; set; }
        [Parameter] public bool Reacted { get; set; }
        [Parameter] public int ReactionsCount { get; set; }

        [Parameter] public EventCallback<ReactionCommandModel> AddReactionCallback { get; set; }
        [Parameter] public EventCallback<ReactionCommandModel> RemoveReactionCallback { get; set; }

        public bool UserIsAuthenticated { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var claimsPrincipal = await GetClaimsPrincipal();
            UserIsAuthenticated = claimsPrincipal.Identity.IsAuthenticated;
        }

        protected async Task AddReaction(PostReactionType type)
        {
            await AddReactionCallback.InvokeAsync(new ReactionCommandModel
            {
                PostReactionType = type,
                PostId = PostId
            });
        }

        protected async Task RemoveReaction(PostReactionType type)
        {
            await RemoveReactionCallback.InvokeAsync(new ReactionCommandModel
            {
                PostReactionType = type,
                PostId = PostId
            });
        }
    }
}