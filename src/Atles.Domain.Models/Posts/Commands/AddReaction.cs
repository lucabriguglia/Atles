using System;
using Atles.Domain.Models.PostReactions;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Models.Posts.Commands
{
    [DocRequest(typeof(PostReactionSummary))]
    public class AddReaction : CommandBase
    {
        public Guid ForumId { get; set; }
        public PostReactionType Type { get; set; }
    }
}
