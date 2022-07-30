using Atles.Core.Queries;
using Atles.Models;
using Atles.Models.Public;

namespace Atles.Queries.Public
{
    public class GetTopicPage : QueryBase<TopicPageModel>
    {
        public string ForumSlug { get; set; }
        public string TopicSlug { get; set; }
        public Guid UserId { get; set; }
        public QueryOptions Options { get; set; }
    }
}
