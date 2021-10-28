using Atles.Infrastructure.Queries;
using Atles.Reporting.Models.Shared;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetTopicPage : QueryBase<TopicPageModel>
    {
        public string ForumSlug { get; set; }
        public string TopicSlug { get; set; }
        public QueryOptions Options { get; set; }
    }
}
