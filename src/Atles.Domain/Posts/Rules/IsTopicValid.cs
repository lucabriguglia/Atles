using Atles.Infrastructure.Queries;
using System;

namespace Atles.Domain.Posts.Rules
{
    public class IsTopicValid : QueryBase<bool>
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
    }
}
