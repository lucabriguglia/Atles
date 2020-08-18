using System.Collections.Generic;
using Atlify.Models.Public;
using Atlify.Domain.PermissionSets;

namespace Atlify.Server.Services
{
    public interface ISecurityService
    {
        bool HasPermission(PermissionModel model);
        bool HasPermission(PermissionType type, IList<PermissionModel> models);
    }
}
