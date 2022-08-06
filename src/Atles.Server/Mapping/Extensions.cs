using Atles.Commands.PermissionSets;
using Atles.Models.Admin.PermissionSets;

namespace Atles.Server.Mapping;

public static class Extensions
{
    public static ICollection<PermissionCommand> ToPermissionCommands(this ICollection<PermissionSetFormModel.PermissionModel> models)
    {
        var result = new List<PermissionCommand>();

        foreach (var permission in models)
        {
            foreach (var permissionType in permission.PermissionTypes)
            {
                if (permissionType.Selected)
                {
                    result.Add(new PermissionCommand
                    {
                        Type = permissionType.Type,
                        RoleId = permission.RoleId
                    });
                }
            }
        }

        return result;
    }
}
