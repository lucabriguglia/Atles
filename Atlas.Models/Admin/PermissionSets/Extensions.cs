using System.Collections.Generic;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.PermissionSets.Commands;

namespace Atlas.Models.Admin.PermissionSets
{
    public static class Extensions
    {
        public static ICollection<PermissionCommand> ToPermissionCommands(this ICollection<FormComponentModel.PermissionModel> models)
        {
            var result = new List<PermissionCommand>();

            foreach (var model in models)
            {
                result.Add(new PermissionCommand
                {
                    Type = model.Type,
                    RoleId = model.RoleId
                });
            }

            return result;
        }

        public static ICollection<FormComponentModel.PermissionModel> ToPermissionModels(this ICollection<Permission> entities)
        {
            var result = new List<FormComponentModel.PermissionModel>();

            foreach (var entity in entities)
            {
                result.Add(new FormComponentModel.PermissionModel
                {
                    Type = entity.Type,
                    RoleId = entity.RoleId
                });
            }

            return result;
        }
    }
}
