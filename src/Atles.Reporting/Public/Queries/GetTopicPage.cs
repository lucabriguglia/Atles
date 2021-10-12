using Atles.Infrastructure.Queries;
using Atles.Models;
using Atles.Models.Public.Topics;

namespace Atles.Reporting.Public.Queries
{
    public class GetTopicPage : QueryBase<TopicPageModel>
    {
        public string ForumSlug { get; set; }
        public string TopicSlug { get; set; }
        public QueryOptions Options { get; set; }
    }
}
