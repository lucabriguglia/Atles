using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Domain.PermissionSets;
using Atlas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Server.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AtlasDbContext _dbContext;
        private readonly IPermissionModelBuilder _permissionModelBuilder;

        public SecurityService(IHttpContextAccessor httpContextAccessor, AtlasDbContext dbContext, IPermissionModelBuilder permissionModelBuilder)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _permissionModelBuilder = permissionModelBuilder;
        }

        public bool HasPermission(PermissionModel model)
        {
            if (model.AllUsers)
            {
                return true;
            }

            foreach (var role in model.Roles)
            {
                if (_httpContextAccessor.HttpContext.User.IsInRole(role.Name))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasPermission(PermissionType permissionType, IList<PermissionModel> models)
        {
            var model = models.FirstOrDefault(x => x.Type == permissionType);

            return model != null && HasPermission(model);
        }

        public async Task<bool> HasPermission(PermissionType permissionType, Guid siteId, Guid forumId)
        {
            var permission = await _dbContext.Forums.Where(x =>
                x.Id == forumId &&
                x.Category.SiteId == siteId &&
                x.Status == StatusType.Published)
                .Select(x => new { Id = x.PermissionSetId ?? x.Category.PermissionSetId })
                .FirstOrDefaultAsync();

            if (permission == null)
            {
                return false;
            }

            var models = await _permissionModelBuilder.BuildPermissionModels(siteId, permission.Id);

            return HasPermission(permissionType, models);
        }
    }
}