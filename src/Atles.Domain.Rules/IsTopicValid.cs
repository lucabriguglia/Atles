using System;
using Atles.Infrastructure.Queries;

namespace Atles.Domain.Rules
{
    public class IsTopicValid : QueryBase<bool>
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
    }
}
