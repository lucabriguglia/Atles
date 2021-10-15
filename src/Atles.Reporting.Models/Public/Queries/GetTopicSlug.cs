using Atles.Infrastructure.Queries;
using System;

namespace Atles.Domain.Posts.Generators
{
    public class GetTopicSlug : QueryBase<string>
    {
        public Guid TopicId { get; set; }
    }
}
