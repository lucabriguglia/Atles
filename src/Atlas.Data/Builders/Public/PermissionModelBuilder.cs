using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.PermissionSets;
using Atlas.Models;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders.Public
{
    public class PermissionModelBuilder : IPermissionModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly RoleManager<SiteRole> _roleManager;

        public PermissionModelBuilder(AtlasDbContext dbContext, 
            ICacheManager cacheManager,
            RoleManager<SiteRole> roleManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _roleManager = roleManager;
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

                var roles = await _roleManager.Roles.ToListAsync();

                foreach (PermissionType permissionType in Enum.GetValues(typeof(PermissionType)))
                {
                    var permissionModel = new PermissionModel
                    {
                        Type = permissionType
                    };

                    var permissions = permissionSet.Permissions.Where(x => x.Type == permissionType);

                    permissionModel.AllUsers = permissions.FirstOrDefault(x => x.RoleId == Consts.RoleIdAll) != null;
                    permissionModel.RegisteredUsers = permissions.FirstOrDefault(x => x.RoleId == Consts.RoleIdRegistered) != null;

                    foreach (var permission in permissions)
                    {
                        var role = roles.FirstOrDefault(x => x.Id == permission.RoleId);

                        if (role != null)
                        {
                            permissionModel.Roles.Add(role.Name);
                        }
                    }

                    result.Add(permissionModel);
                }

                return result;
            });
        }

        public async Task<IList<PermissionModel>> BuildPermissionModelsByForumId(Guid siteId, Guid forumId)
        {
            var permission = await _dbContext.Forums.Where(x =>
                    x.Id == forumId &&
                    x.Category.SiteId == siteId &&
                    x.Status == StatusType.Published)
                .Select(x => new { Id = x.PermissionSetId ?? x.Category.PermissionSetId })
                .FirstOrDefaultAsync();

            if (permission == null)
            {
                return new List<PermissionModel>();
            }

            return await BuildPermissionModels(siteId, permission.Id);
        }
    }
}