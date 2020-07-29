using System.Collections.Generic;
using System.Linq;
using Atlas.Domain.PermissionSets;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Http;

namespace Atlas.Server.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecurityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool HasPermission(PermissionModel model)
        {
            if (model.AllUsers)
            {
                return true;
            }

            if (model.RegisteredUsers)
            {
                return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
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
    }
}