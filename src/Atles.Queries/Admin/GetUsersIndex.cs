using Atles.Core.Queries;
using Atles.Models;
using Atles.Models.Admin.Users;

namespace Atles.Queries.Admin
{
    public class GetUsersIndex : QueryBase<IndexPageModel>
    {
        public QueryOptions Options { get; set; }
        public string Status { get; set; }
    }
}
