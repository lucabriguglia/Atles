using Atles.Data;
using Atles.Domain;
using Atles.Domain.PermissionSets;
using Atles.Models.Admin.PermissionSets;
using Atles.Models.Admin.PermissionSets.Queries;
using Atles.Models.Admin.Roles;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Admin.PermissionSets
{
    public class GetPermissionSetEditFormHandler : IQueryHandler<GetPermissionSetEditForm, FormComponentModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IRoleModelBuilder _roleManager;

        public GetPermissionSetEditFormHandler(AtlesDbContext dbContext, IRoleModelBuilder roleManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
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

            foreach (var roleModel in await _roleManager.GetRoleModelsAsync())
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
