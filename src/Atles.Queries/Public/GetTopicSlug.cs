using System;
using Atles.Core.Queries;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetTopicSlug : QueryBase<string>
    {
        public Guid TopicId { get; set; }
    }
}
