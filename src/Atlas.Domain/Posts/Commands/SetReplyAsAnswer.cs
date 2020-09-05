using System;
using Docs.Attributes;

namespace Atlas.Domain.Posts.Commands
{
    [DocRequest(typeof(Post))]
    public class SetReplyAsAnswer : CommandBase
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public Guid TopicId { get; set; }
        public bool IsAnswer { get; set; }
    }
}