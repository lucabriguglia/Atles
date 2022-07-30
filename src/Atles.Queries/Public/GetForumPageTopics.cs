using Atles.Core.Queries;
using Atles.Models;
using Atles.Models.Public;

namespace Atles.Queries.Public
{
    public class GetForumPageTopics : QueryBase<PaginatedData<ForumPageModel.TopicModel>>
    {
        public Guid ForumId { get; set; }
        public QueryOptions Options { get; set; }
    }
}
