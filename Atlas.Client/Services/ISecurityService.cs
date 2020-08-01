using System.Collections.Generic;
using System.Security.Claims;
using Atlas.Domain.PermissionSets;
using Atlas.Models;

namespace Atlas.Client.Services
{
    public interface ISecurityService
    {
        bool HasPermission(ClaimsPrincipal user, PermissionModel model);
        bool HasPermission(ClaimsPrincipal user, PermissionType type, IList<PermissionModel> models);
    }
}
