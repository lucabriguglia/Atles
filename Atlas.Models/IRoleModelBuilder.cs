using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Models
{
    public interface IRoleModelBuilder
    {
        Task<IList<RoleModel>> GetRoleModels();
    }
}
