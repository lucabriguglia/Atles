using System;
using Atles.Domain.Models.PostReactions;
using Atles.Domain.PostReactions;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    [DocRequest(typeof(PostReaction))]
    public class AddReaction : CommandBase
    {
        public Guid PostId { get; set; }
        public PostReactionType Type { get; set; }
    }
}
