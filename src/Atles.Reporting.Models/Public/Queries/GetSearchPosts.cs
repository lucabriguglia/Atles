using Atles.Infrastructure.Queries;
using Atles.Models;
using Atles.Models.Public.Search;
using System;
using System.Collections.Generic;

namespace Atles.Reporting.Public.Queries
{
    public class GetSearchPosts : QueryBase<PaginatedData<SearchPostModel>>
    {
        public IEnumerable<Guid> AccessibleForumIds { get; set; }
        public Guid? UserId { get; set; }
        public QueryOptions Options { get; set; }
    }
}
