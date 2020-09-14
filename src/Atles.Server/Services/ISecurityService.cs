using System.Collections.Generic;
using Atles.Domain.PermissionSets;
using Atles.Models.Public;

namespace Atlas.Server.Services
{
    public interface ISecurityService
    {
        bool HasPermission(PermissionModel model);
        bool HasPermission(PermissionType type, IList<PermissionModel> models);
    }
}
