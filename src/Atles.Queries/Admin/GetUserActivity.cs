using Atles.Core.Queries;
using Atles.Models;
using Atles.Models.Admin.Users;

namespace Atles.Queries.Admin
{
    public class GetUserActivity : QueryBase<ActivityPageModel>
    {
        public QueryOptions Options { get; set; }
        public Guid UserId { get; set; }
    }
}
