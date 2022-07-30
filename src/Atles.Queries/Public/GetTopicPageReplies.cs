using Atles.Core.Queries;
using Atles.Models;
using Atles.Models.Public;

namespace Atles.Queries.Public
{
    public class GetTopicPageReplies : QueryBase<PaginatedData<TopicPageModel.ReplyModel>>
    {
        public Guid TopicId { get; set; }
        public QueryOptions Options { get; set; }
    }
}
