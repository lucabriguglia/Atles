using Atles.Core.Queries;
using Atles.Models;
using Atles.Models.Public;

namespace Atles.Queries.Public
{
    public class GetSearchPage : QueryBase<SearchPageModel>
    {
        public IEnumerable<Guid> AccessibleForumIds { get; set; }
        public QueryOptions Options { get; set; }
    }
}
