using Atles.Infrastructure.Queries;
using System;

namespace Atles.Domain.Posts.Generators
{
    public class GenerateTopicSlug : QueryBase<string>
    {
        public Guid ForumId { get; set; }
        public string Title { get; set; }
    }
}
