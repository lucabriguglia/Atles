using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Public;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public
{
    public class GetPermissionsHandler : IQueryHandler<GetPermissions, IList<PermissionModel>>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetPermissionsHandler(AtlesDbContext dbContext, ICacheManager cacheManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _roleManager = roleManager;
        }

        public async Task<QueryResult<IList<PermissionModel>>> Handle(GetPermissions query)
        {
            if (query.ForumId != null)
            {
                var permission = await _dbContext.Forums.Where(x =>
                        x.Id == query.ForumId &&
                        x.Category.SiteId == query.SiteId &&
                        x.Status == ForumStatusType.Published)
                    .Select(x => new { Id = x.PermissionSetId ?? x.Category.PermissionSetId })
                    .FirstOrDefaultAsync();

                if (permission != null)
                {
                    return await BuildPermissionModels(query.SiteId, permission.Id);
                }
            }
            else if(query.PermissionSetId != null)
            {
                return await BuildPermissionModels(query.SiteId, query.PermissionSetId.Value);
            }

            return new List<PermissionModel>();
        }

        private async Task<QueryResult<IList<PermissionModel>>> BuildPermissionModels(Guid siteId, Guid permissionSetId)
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
}
