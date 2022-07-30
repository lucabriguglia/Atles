using Atles.Domain;

namespace Atles.Commands.PermissionSets
{
    public static class PermissionCommandExtensions
    {
        public static ICollection<Permission> ToDomainPermissions(this ICollection<PermissionCommand> models)
        {
            var result = new List<Permission>();

            foreach (var permission in models)
            {
                result.Add(new Permission(permission.Type, permission.RoleId));
            }

            return result;
        }
    }
}
