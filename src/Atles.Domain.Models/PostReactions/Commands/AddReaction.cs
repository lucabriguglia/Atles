using Atles.Infrastructure.Commands;
using Docs.Attributes;
using System;
using Atles.Domain.Models.PostReactions;

namespace Atles.Domain.PostReactions.Commands
{
    [DocRequest(typeof(PostReaction))]
    public class AddReaction : CommandBase
    {
        public Guid PostId { get; set; }
        public PostReactionType Type { get; set; }
    }
}
