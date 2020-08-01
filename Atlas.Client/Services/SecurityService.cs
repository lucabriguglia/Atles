using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Atlas.Domain.PermissionSets;
using Atlas.Models;

namespace Atlas.Client.Services
{
    public class SecurityService : ISecurityService
    {
        public bool HasPermission(ClaimsPrincipal user, PermissionModel model)
        {
            if (model.AllUsers)
            {
                return true;
            }

            if (model.RegisteredUsers)
            {
                return user.Identity.IsAuthenticated;
            }

            foreach (var role in model.Roles)
            {
                if (user.IsInRole(role))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasPermission(ClaimsPrincipal user, PermissionType permissionType, IList<PermissionModel> models)
        {
            var model = models.FirstOrDefault(x => x.Type == permissionType);

            return model != null && HasPermission(user, model);
        }
    }
}