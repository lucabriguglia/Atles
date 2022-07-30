using System;
using Atles.Core.Queries;
using Atles.Reporting.Models.Shared;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetForumPageTopics : QueryBase<PaginatedData<ForumPageModel.TopicModel>>
    {
        public Guid ForumId { get; set; }
        public QueryOptions Options { get; set; }
    }
}
