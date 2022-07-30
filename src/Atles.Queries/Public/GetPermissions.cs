using System;
using System.Collections.Generic;
using Atles.Core.Queries;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetPermissions : QueryBase<IList<PermissionModel>>
    {
        public Guid? PermissionSetId { get; set; }
        public Guid? ForumId { get; set; }
    }
}
