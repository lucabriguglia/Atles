using Atles.Infrastructure.Commands;
using Docs.Attributes;
using System;

namespace Atles.Domain.PostReactions.Commands
{
    [DocRequest(typeof(PostReaction))]
    public class RemoveReaction : CommandBase
    {
        public Guid PostId { get; set; }
    }
}
