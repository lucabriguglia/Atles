using Atles.Core;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Domain;
using Atles.Models.Admin.PermissionSets;
using Atles.Queries.Admin;

namespace Atles.Queries.Handlers.Admin;

public class GetPermissionSetCreateFormHandler : IQueryHandler<GetPermissionSetCreateForm, PermissionSetFormModel>
{
    private readonly IDispatcher _dispatcher;

    public GetPermissionSetCreateFormHandler(IDispatcher sender)
    {
        _dispatcher = sender;
    }

    public async Task<QueryResult<PermissionSetFormModel>> Handle(GetPermissionSetCreateForm query)
    {
        var result = new PermissionSetFormModel();

        // TODO: To be moved to a service
        var queryResult = await _dispatcher.Get(new GetRoles());
        var roles = queryResult.AsT0;

        foreach (var roleModel in roles)
        {
            var permissionModel = new PermissionSetFormModel.PermissionModel
            {
                RoleId = roleModel.Id,
                RoleName = roleModel.Name
            };

            foreach (PermissionType permissionType in Enum.GetValues(typeof(PermissionType)))
            {
                var disabled = roleModel.Name == Consts.RoleNameAdmin ||
                               roleModel.Id == Consts.RoleIdAll && !IsReadingPermissionType(permissionType);

                permissionModel.PermissionTypes.Add(new PermissionSetFormModel.PermissionTypeModel
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
