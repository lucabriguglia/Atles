using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.PermissionSets;
using Atlas.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders
{
    public class PermissionModelBuilder : IPermissionModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IRoleModelBuilder _roles;

        public PermissionModelBuilder(AtlasDbContext dbContext, ICacheManager cacheManager, IRoleModelBuilder roles)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _roles = roles;
        }

        public async Task<IList<PermissionModel>> BuildPermissionModels(Guid siteId, Guid permissionSetId)
        {
            return await _cacheManager.GetOrSetAsync(CacheKeys.PermissionSet(permissionSetId), async () =>
            {
                var result = new List<PermissionModel>();

                var permissionSet = await _dbContext.PermissionSets
                    .Include(x => x.Permissions)
                    .FirstOrDefaultAsync(x =>
                        x.SiteId == siteId &&
                        x.Id == permissionSetId &&
                        x.Status != StatusType.Deleted);

                if (permissionSet == null)
                {
                    return result;
                }

                var roles = await _roles.GetRoleModels();

                foreach (PermissionType permissionType in Enum.GetValues(typeof(PermissionType)))
                {
                    var permissionModel = new PermissionModel
                    {
                        Type = permissionType
                    };

                    var permissions = permissionSet.Permissions.Where(x => x.Type == permissionType);

                    permissionModel.AllUsers = permissions.FirstOrDefault(x => x.RoleId == Consts.RoleIdAll) != null;

                    foreach (var permission in permissions)
                    {
                        var role = roles.FirstOrDefault(x => x.Id == permission.RoleId);

                        if (role != null)
                        {
                            permissionModel.Roles.Add(new RoleModel
                            {
                                Id = role.Id,
                                Name = role.Name
                            });
                        }
                    }

                    result.Add(permissionModel);
                }

                return result;
            });
        }
    }
}