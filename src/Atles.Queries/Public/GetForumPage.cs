using Atles.Core.Queries;
using Atles.Reporting.Models.Shared;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetForumPage : QueryBase<ForumPageModel>
    {
        public string Slug { get; set; }
        public QueryOptions Options { get; set; }
    }
}
