using System;
using Docs.Attributes;

namespace Atlas.Domain.Posts.Commands
{
    [DocRequest(typeof(Post))]
    public class PinTopic : CommandBase
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public bool Pinned { get; set; }
    }
}