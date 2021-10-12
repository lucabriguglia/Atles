using Atles.Infrastructure.Queries;
using Atles.Models;
using Atles.Models.Public.Topics;
using System;

namespace Atles.Reporting.Public.Queries
{
    public class GetTopicPageReplies : QueryBase<PaginatedData<TopicPageModel.ReplyModel>>
    {
        public Guid TopicId { get; set; }
        public QueryOptions Options { get; set; }
    }
}
