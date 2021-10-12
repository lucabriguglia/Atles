using Atles.Domain;
using Atles.Domain.PermissionSets;
using Atles.Models.Admin.PermissionSets;
using Atles.Models.Admin.PermissionSets.Queries;
using Atles.Reporting.Admin.Roles.Queries;
using OpenCqrs;
using OpenCqrs.Queries;
using System;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Admin.PermissionSets
{
    public class GetPermissionSetCreateFormHandler : IQueryHandler<GetPermissionSetCreateForm, FormComponentModel>
    {
        private readonly ISender _sender;

        public GetPermissionSetCreateFormHandler(ISender sender)
        {
            _sender = sender;
        }

        public async Task<FormComponentModel> Handle(GetPermissionSetCreateForm query)
        {
            var result = new FormComponentModel();

            foreach (var roleModel in await _sender.Send(new GetRoles()))
            {
                var permissionModel = new FormComponentModel.PermissionModel
                {
                    RoleId = roleModel.Id,
                    RoleName = roleModel.Name
                };

                foreach (PermissionType permissionType in Enum.GetValues(typeof(PermissionType)))
                {
                    var disabled = roleModel.Name == Consts.RoleNameAdmin ||
                                   roleModel.Id == Consts.RoleIdAll && !IsReadingPermissionType(permissionType);

                    permissionModel.PermissionTypes.Add(new FormComponentModel.PermissionTypeModel
                    {
                        Type = permissionType,
                        Selected = roleModel.Name == Consts.RoleNameAdmin,
                        Disabled = disabled
                    });
                }

                result.PermissionSet.Permissions.Add(permissionModel);
            }

            return result;
        }

        private static bool IsReadingPermissionType(PermissionType permissionType) =>
            permissionType == PermissionType.ViewForum ||
            permissionType == PermissionType.ViewTopics ||
            permissionType == PermissionType.Read;
    }
}
