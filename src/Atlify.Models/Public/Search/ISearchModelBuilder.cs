using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlify.Models.Public.Search
{
    public interface ISearchModelBuilder
    {
        Task<SearchPageModel> BuildSearchPageModelAsync(Guid siteId, IList<Guid> forumIds, QueryOptions options);

        Task<PaginatedData<SearchPostModel>> SearchPostModels(IList<Guid> forumIds, QueryOptions options, Guid? memberId = null);
    }
}