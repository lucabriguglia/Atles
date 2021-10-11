using Atles.Infrastructure.Queries;
using Atles.Models;
using Atles.Models.Public.Forums;

namespace Atles.Reporting.Public.Queries
{
    public class GetForumPage : QueryBase<ForumPageModel>
    {
        public string Slug { get; set; }
        public QueryOptions Options { get; set; }
    }
}
