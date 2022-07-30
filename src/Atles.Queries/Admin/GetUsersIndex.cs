using Atles.Core.Queries;
using Atles.Reporting.Models.Shared;

namespace Atles.Reporting.Models.Admin.Users.Queries
{
    public class GetUsersIndex : QueryBase<IndexPageModel>
    {
        public QueryOptions Options { get; set; }
        public string Status { get; set; }
    }
}
