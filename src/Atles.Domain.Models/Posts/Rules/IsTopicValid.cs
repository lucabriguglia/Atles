using System;
using Atles.Infrastructure.Queries;

namespace Atles.Domain.Models.Posts.Rules
{
    public class IsTopicValid : QueryBase<bool>
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
    }
}
