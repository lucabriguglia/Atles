using System;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.PermissionSets;
using Atles.Reporting.Models.Admin.PermissionSets;
using Atles.Reporting.Models.Admin.PermissionSets.Queries;
using Atles.Reporting.Models.Admin.Roles.Queries;
using OpenCqrs;
using OpenCqrs.Queries;

namespace Atles.Reporting.Handlers.Admin
{
    public class GetPermissionSetCreateFormHandler : IQueryHandler<GetPermissionSetCreateForm, FormComponentModel>
    {
        private readonly IDispatcher _dispatcher;

        public GetPermissionSetCreateFormHandler(IDispatcher sender)
        {
            _dispatcher = sender;
        }

        public async Task<FormComponentModel> Handle(GetPermissionSetCreateForm query)
        {
            var result = new FormComponentModel();

            foreach (var roleModel in await _dispatcher.Get(new GetRoles()))
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
