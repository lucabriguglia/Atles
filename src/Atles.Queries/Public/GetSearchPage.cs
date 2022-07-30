using System;
using System.Collections.Generic;
using Atles.Core.Queries;
using Atles.Reporting.Models.Shared;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetSearchPage : QueryBase<SearchPageModel>
    {
        public IEnumerable<Guid> AccessibleForumIds { get; set; }
        public QueryOptions Options { get; set; }
    }
}
