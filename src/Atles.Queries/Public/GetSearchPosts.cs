using System;
using System.Collections.Generic;
using Atles.Core.Queries;
using Atles.Reporting.Models.Shared;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetSearchPosts : QueryBase<PaginatedData<SearchPostModel>>
    {
        public IEnumerable<Guid> AccessibleForumIds { get; set; }
        public Guid? UserId { get; set; }
        public QueryOptions Options { get; set; }
    }
}
