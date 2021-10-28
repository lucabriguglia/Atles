using System;
using Atles.Infrastructure.Queries;
using Atles.Reporting.Models.Shared;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetTopicPageReplies : QueryBase<PaginatedData<TopicPageModel.ReplyModel>>
    {
        public Guid TopicId { get; set; }
        public QueryOptions Options { get; set; }
    }
}
