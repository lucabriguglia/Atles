using Atles.Core;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Admin.PermissionSets;
using Atles.Queries.Admin;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Admin;

public class GetPermissionSetEditFormHandler : IQueryHandler<GetPermissionSetEditForm, PermissionSetFormModel>
{
    private readonly AtlesDbContext _dbContext;
    private readonly IDispatcher _dispatcher;

    public GetPermissionSetEditFormHandler(AtlesDbContext dbContext, IDispatcher sender)
    {
        _dbContext = dbContext;
        _dispatcher = sender;
    }

    public async Task<QueryResult<PermissionSetFormModel>> Handle(GetPermissionSetEditForm query)
    {
        var result = new PermissionSetFormModel();

        var permissionSet = await _dbContext.PermissionSets
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x =>
                x.SiteId == query.SiteId &&
                x.Id == query.Id &&
                x.Status != PermissionSetStatusType.Deleted);

        if (permissionSet == null)
        {
            return new Failure(FailureType.NotFound, "Permission Set", $"Permission set with id {query.Id} not found.");
        }

        result.PermissionSet = new PermissionSetFormModel.PermissionSetModel
        {
            Id = permissionSet.Id,
            Name = permissionSet.Name
        };

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
                var selected = permissionSet.Permissions
                                   .FirstOrDefault(x => x.Type == permissionType &&
                                                        x.RoleId == roleModel.Id) != null
                               || roleModel.Name == Consts.RoleNameAdmin;

                var disabled = roleModel.Name == Consts.RoleNameAdmin ||
                               roleModel.Id == Consts.RoleIdAll && !IsReadingPermissionType(permissionType);

                permissionModel.PermissionTypes.Add(new PermissionSetFormModel.PermissionTypeModel
                {
                    Type = permissionType,
                    Selected = selected,
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
