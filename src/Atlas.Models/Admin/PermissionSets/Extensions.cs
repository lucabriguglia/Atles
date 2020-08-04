using System.Collections.Generic;
using Atlas.Domain.PermissionSets.Commands;

namespace Atlas.Models.Admin.PermissionSets
{
    public static class Extensions
    {
        public static ICollection<PermissionCommand> ToPermissionCommands(this ICollection<FormComponentModel.PermissionModel> models)
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
}
