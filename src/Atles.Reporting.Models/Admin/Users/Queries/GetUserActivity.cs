using Atles.Infrastructure.Queries;
using Atles.Models;
using Atles.Models.Admin.Users;
using System;

namespace Atles.Reporting.Admin.Users.Queries
{
    public class GetUserActivity : QueryBase<ActivityPageModel>
    {
        public QueryOptions Options { get; set; }
        public Guid UserId { get; set; }
    }
}
