using System;
using Atles.Domain.Models.PostReactions;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    [DocRequest(typeof(PostReaction))]
    public class RemoveReaction : CommandBase
    {
        public Guid PostId { get; set; }
    }
}
