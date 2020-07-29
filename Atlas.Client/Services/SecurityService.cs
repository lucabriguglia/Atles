using System.Collections.Generic;
using System.Linq;
using Atlas.Domain.PermissionSets;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components.Authorization;

namespace Atlas.Client.Services
{
    public class SecurityService : ISecurityService
    {
        public bool HasPermission(AuthenticationState authenticationState, PermissionModel model)
        {
            if (model.AllUsers)
            {
                return true;
            }

            if (model.RegisteredUsers)
            {
                return authenticationState.User.Identity.IsAuthenticated;
            }

            foreach (var role in model.Roles)
            {
                if (authenticationState.User.IsInRole(role.Name))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasPermission(AuthenticationState authenticationState, PermissionType permissionType, IList<PermissionModel> models)
        {
            var model = models.FirstOrDefault(x => x.Type == permissionType);

            return model != null && HasPermission(authenticationState, model);
        }
    }
}