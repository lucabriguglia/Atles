using System;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Models.Posts.Commands
{
    [DocRequest(typeof(PostReactionSummary))]
    public class RemoveReaction : CommandBase
    {
        public Guid ForumId { get; set; }
    }
}
