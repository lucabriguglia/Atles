using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Domain.PermissionSets;
using Atlas.Models;

namespace Atlas.Server.Services
{
    public interface ISecurityService
    {
        bool HasPermission(PermissionModel model);
        bool HasPermission(PermissionType type, IList<PermissionModel> models);
        [Obsolete("Use IPermissionModelBuilder.BuildPermissionModelsByForumId instead to get permissions and then call ISecurityService.HasPermission.")]
        Task<bool> HasPermission(PermissionType permissionType, Guid siteId, Guid forumId);
    }
}
