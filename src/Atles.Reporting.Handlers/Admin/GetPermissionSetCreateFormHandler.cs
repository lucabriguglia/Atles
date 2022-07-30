using System;
using System.Threading.Tasks;
using Atles.Core;
using Atles.Core.Queries;
using Atles.Domain;
using Atles.Models.Admin.PermissionSets;
using Atles.Queries.Admin;

namespace Atles.Queries.Handlers.Admin
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
