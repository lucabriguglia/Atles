using Atles.Core.Queries;
using Atles.Models;
using Atles.Models.Public;

namespace Atles.Queries.Public
{
    public class GetSearchPosts : QueryBase<PaginatedData<SearchPostModel>>
    {
        public IEnumerable<Guid> AccessibleForumIds { get; set; }
        public Guid? UserId { get; set; }
        public QueryOptions Options { get; set; }
    }
}
