using Atles.Infrastructure.Queries;
using Atles.Models.Admin.Roles;
using System.Collections.Generic;

namespace Atles.Reporting.Admin.Roles.Queries
{
    public class GetUsersInRole : QueryBase<IList<IndexPageModel.UserModel>>
    {
        public string RoleName { get; set; }
    }
}
