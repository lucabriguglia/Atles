using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Models
{
    public interface IPermissionModelBuilder
    {
        Task<IList<PermissionModel>> BuildPermissionModels(Guid siteId, Guid permissionSetId);
    }
}