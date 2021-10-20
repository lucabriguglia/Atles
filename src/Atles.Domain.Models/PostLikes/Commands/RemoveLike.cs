using Atles.Infrastructure.Commands;
using Docs.Attributes;
using System;

namespace Atles.Domain.PostLikes.Commands
{
    [DocRequest(typeof(PostLike))]
    public class RemoveLike : CommandBase
    {
        public Guid PostId { get; set; }
    }
}
