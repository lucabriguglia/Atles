using System.Collections.Generic;
using Atlas.Domain.PermissionSets;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components.Authorization;

namespace Atlas.Client.Services
{
    public interface ISecurityService
    {
        bool HasPermission(AuthenticationState authenticationState, PermissionModel model);
        bool HasPermission(AuthenticationState authenticationState, PermissionType type, IList<PermissionModel> models);
    }
}
