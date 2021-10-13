using Atles.Infrastructure.Queries;
using Atles.Models.Public;
using System;
using System.Collections.Generic;

namespace Atles.Reporting.Public.Queries
{
    public class GetPermissions : QueryBase<IList<PermissionModel>>
    {
        public Guid? PermissionSetId { get; set; }
        public Guid? ForumId { get; set; }
    }
}
