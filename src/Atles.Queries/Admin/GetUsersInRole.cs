using Atles.Core.Queries;
using Atles.Models.Admin.Roles;

namespace Atles.Queries.Admin;

public class GetUsersInRole : QueryBase<IList<IndexPageModel.UserModel>>
{
    public string RoleName { get; set; }
}
