using Atles.Core.Queries;
using Atles.Models;
using Atles.Models.Public;

namespace Atles.Queries.Public
{
    public class GetForumPage : QueryBase<ForumPageModel>
    {
        public string Slug { get; set; }
        public QueryOptions Options { get; set; }
    }
}
