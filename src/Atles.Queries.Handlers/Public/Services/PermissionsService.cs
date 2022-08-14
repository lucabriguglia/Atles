﻿using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Models.Public;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public.Services;

public class PermissionsService : IPermissionsService
{
    private readonly AtlesDbContext _dbContext;
    private readonly ICacheManager _cacheManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public PermissionsService(AtlesDbContext dbContext, ICacheManager cacheManager, RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _cacheManager = cacheManager;
        _roleManager = roleManager;
    }

    public async Task<IList<PermissionModel>> GetPermissions(Guid siteId, Guid? forumId, Guid? permissionSetId)
    {
        if (forumId != null)
        {
            var permission = await _dbContext.Forums.Where(x =>
                    x.Id == forumId &&
                    x.Category.SiteId == siteId &&
                    x.Status == ForumStatusType.Published)
                .Select(x => new { Id = x.PermissionSetId ?? x.Category.PermissionSetId })
                .FirstOrDefaultAsync();

            if (permission != null)
            {
                return await BuildPermissionModels(siteId, permission.Id);
            }
        }
        else if (permissionSetId != null)
        {
            return await BuildPermissionModels(siteId, permissionSetId.Value);
        }

        return new List<PermissionModel>();
    }

    private async Task<IList<PermissionModel>> BuildPermissionModels(Guid siteId, Guid permissionSetId)
    {
        return await _cacheManager.GetOrSetAsync(CacheKeys.PermissionSet(permissionSetId), async () =>
        {
            var result = new List<PermissionModel>();

            var permissionSet = await _dbContext.PermissionSets
                .Include(x => x.Permissions)
                .FirstOrDefaultAsync(x =>
                    x.SiteId == siteId &&
                    x.Id == permissionSetId &&
                    x.Status != PermissionSetStatusType.Deleted);

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
}