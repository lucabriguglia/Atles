using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atles.Models.Public
{
    public interface IPermissionModelBuilder
    {
        Task<IList<PermissionModel>> BuildPermissionModels(Guid siteId, Guid permissionSetId);
        Task<IList<PermissionModel>> BuildPermissionModelsByForumId(Guid siteId, Guid forumId);
    }
}