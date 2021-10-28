using System;
using System.Collections.Generic;
using Atles.Infrastructure.Queries;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetUserPage : QueryBase<UserPageModel>
    {
        public Guid UserId { get; set; }
        public IEnumerable<Guid> AccessibleForumIds { get; set; }
    }
}
