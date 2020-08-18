using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Domain.PermissionSets;
using Atlas.Models.Admin.PermissionSets;
using Atlas.Models.Admin.Roles;
using IndexPageModel = Atlas.Models.Admin.PermissionSets.IndexPageModel;

namespace Atlas.Data.Builders.Admin
{
    public class PermissionSetModelBuilder : IPermissionSetModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly IRoleModelBuilder _roleManager;

        public PermissionSetModelBuilder(AtlasDbContext dbContext, IRoleModelBuilder roleManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
        }

        public async Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId)
        {
            var result = new IndexPageModel();

            var permissionSets = await _dbContext.PermissionSets
                .Include(x => x.Categories)
                .Include(x => x.Forums)
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new IndexPageModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name,
                    IsInUse = permissionSet.Categories.Any() || permissionSet.Forums.Any()
                });
            }

            return result;
        }

        public async Task<FormComponentModel> BuildCreateFormModelAsync(Guid siteId)
        {
            var result = new FormComponentModel();

            foreach (var roleModel in await _roleManager.GetRoleModelsAsync())
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

        public async Task<FormComponentModel> BuildEditFormModelAsync(Guid siteId, Guid id)
        {
            var result = new FormComponentModel();

            var permissionSet = await _dbContext.PermissionSets
                .Include(x => x.Permissions)
                .FirstOrDefaultAsync(x => 
                    x.SiteId == siteId && 
                    x.Id == id && 
                    x.Status != StatusType.Deleted);

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
