using System.Collections.Generic;
using Atlas.Models.Public;
using Atles.Domain.PermissionSets;

namespace Atlas.Server.Services
{
    public interface ISecurityService
    {
        bool HasPermission(PermissionModel model);
        bool HasPermission(PermissionType type, IList<PermissionModel> models);
    }
}
