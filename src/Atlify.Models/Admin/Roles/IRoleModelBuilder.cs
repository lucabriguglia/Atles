using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlify.Models.Admin.Roles
{
    public interface IRoleModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync();
        Task<IList<IndexPageModel.UserModel>> BuildUsersInRoleModelsAsync(string roleName);
        Task<IList<RoleModel>> GetRoleModelsAsync();
    }
}
