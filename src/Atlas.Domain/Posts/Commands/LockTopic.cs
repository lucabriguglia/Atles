using System;
using Docs.Attributes;

namespace Atlas.Domain.Posts.Commands
{
    [DocRequest(typeof(Post))]
    public class LockTopic : CommandBase
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public bool Locked { get; set; }
    }
}