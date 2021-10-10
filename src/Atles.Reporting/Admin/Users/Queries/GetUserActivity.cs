using Atles.Infrastructure.Queries;
using Atles.Models;
using Atles.Models.Admin.Users;

namespace Atles.Reporting.Admin.Users.Queries
{
    public class GetUserActivity : QueryBase<ActivityPageModel>
    {
        public QueryOptions QueryOptions { get; set; }
        public string Status { get; set; }
    }
}
