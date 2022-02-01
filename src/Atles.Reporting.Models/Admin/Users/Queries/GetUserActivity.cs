using System;
using Atles.Core.Queries;
using Atles.Reporting.Models.Shared;

namespace Atles.Reporting.Models.Admin.Users.Queries
{
    public class GetUserActivity : QueryBase<ActivityPageModel>
    {
        public QueryOptions Options { get; set; }
        public Guid UserId { get; set; }
    }
}
