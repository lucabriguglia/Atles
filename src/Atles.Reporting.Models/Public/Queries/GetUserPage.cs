using Atles.Infrastructure.Queries;
using Atles.Models.Public.Users;
using System;
using System.Collections.Generic;

namespace Atles.Reporting.Public.Queries
{
    public class GetUserPage : QueryBase<UserPageModel>
    {
        public Guid UserId { get; set; }
        public IEnumerable<Guid> AccessibleForumIds { get; set; }
    }
}
