using System.Security.Claims;
using Atles.Domain;
using Atles.Models.Public;

namespace Atles.Server.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool HasPermission(this ClaimsPrincipal claimsPrincipal, PermissionModel permissionModel)
    {
        if (permissionModel.AllUsers)
        {
            return true;
        }

        if (permissionModel.RegisteredUsers)
        {
            return claimsPrincipal.Identity?.IsAuthenticated ?? false;
        }

        return permissionModel.Roles.Any(claimsPrincipal.IsInRole);
    }

    public static bool HasPermission(this ClaimsPrincipal claimsPrincipal, IEnumerable<PermissionModel> permissionModels, PermissionType permissionType)
    {
        var permissionModel = permissionModels.FirstOrDefault(x => x.Type == permissionType);
        return permissionModel != null && claimsPrincipal.HasPermission(permissionModel);
    }
}
