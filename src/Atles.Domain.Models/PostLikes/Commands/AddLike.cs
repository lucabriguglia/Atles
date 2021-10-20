using Atles.Infrastructure.Commands;
using Docs.Attributes;
using System;

namespace Atles.Domain.PostLikes.Commands
{
    [DocRequest(typeof(PostLike))]
    public class AddLike : CommandBase
    {
        public Guid PostId { get; set; }
        public bool Like { get; set; } = true;
    }
}
