using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Domain.PermissionSets;
using Atlas.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Atlas.Client.Services
{
    public class SecurityService : ISecurityService
    {
        public async Task<bool> HasPermission(Task<AuthenticationState> authenticationStateTask, PermissionModel model)
        {
            var authenticationState = await authenticationStateTask;

            if (model.AllUsers)
            {
                return true;
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

        public async Task<bool> HasPermission(Task<AuthenticationState> authenticationStateTask, PermissionType permissionType, IList<PermissionModel> models)
        {
            var model = models.FirstOrDefault(x => x.Type == permissionType);

            return model != null && await HasPermission(authenticationStateTask, model);
        }
    }
}