using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Models;
using Atles.Infrastructure;
using Atles.Infrastructure.Queries;
using Atles.Reporting.Models.Admin.PermissionSets;
using Atles.Reporting.Models.Admin.PermissionSets.Queries;
using Atles.Reporting.Models.Admin.Roles.Queries;
using Microsoft.EntityFrameworkCore;

namespace Atles.Reporting.Handlers.Admin
{
    public class GetPermissionSetEditFormHandler : IQueryHandler<GetPermissionSetEditForm, FormComponentModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IDispatcher _dispatcher;

        public GetPermissionSetEditFormHandler(AtlesDbContext dbContext, IDispatcher sender)
        {
            _dbContext = dbContext;
            _dispatcher = sender;
        }

        public async Task<FormComponentModel> Handle(GetPermissionSetEditForm query)
        {
            var result = new FormComponentModel();

            var permissionSet = await _dbContext.PermissionSets
                .Include(x => x.Permissions)
                .FirstOrDefaultAsync(x =>
                    x.SiteId == query.SiteId &&
                    x.Id == query.Id &&
                    x.Status != PermissionSetStatusType.Deleted);

            if (permissionSet == null)
            {
                return null;
            }

            result.PermissionSet = new FormComponentModel.PermissionSetModel
            {
                Id = permissionSet.Id,
                Name = permissionSet.Name
            };

            foreach (var roleModel in await _dispatcher.Get(new GetRoles()))
            {
                var permissionModel = new FormComponentModel.PermissionModel
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

                    permissionModel.PermissionTypes.Add(new FormComponentModel.PermissionTypeModel
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
}
