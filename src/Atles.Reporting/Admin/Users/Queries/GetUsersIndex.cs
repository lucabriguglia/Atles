using Atles.Infrastructure.Queries;
using Atles.Models;
using Atles.Models.Admin.Users;

namespace Atles.Reporting.Admin.Users.Queries
{
    public class GetUsersIndex : QueryBase<IndexPageModel>
    {
        public QueryOptions Options { get; set; }
        public string Status { get; set; }
    }
}
