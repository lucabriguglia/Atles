using System;
using Atles.Domain.Models.PostReactions;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Models.Posts.Commands
{
    [DocRequest(typeof(PostReaction))]
    public class AddReaction : CommandBase
    {
        public PostReactionType Type { get; set; }
    }
}
