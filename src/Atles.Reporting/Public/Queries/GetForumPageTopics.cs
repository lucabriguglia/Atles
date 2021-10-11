using Atles.Infrastructure.Queries;
using Atles.Models;
using Atles.Models.Public.Forums;
using System;

namespace Atles.Reporting.Public.Queries
{
    public class GetForumPageTopics : QueryBase<PaginatedData<ForumPageModel.TopicModel>>
    {
        public Guid ForumId { get; set; }
        public QueryOptions Options { get; set; }
    }
}
