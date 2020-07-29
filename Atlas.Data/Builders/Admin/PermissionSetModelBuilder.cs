using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Domain.PermissionSets;
using Atlas.Models.Admin.PermissionSets;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Data.Builders.Admin
{
    public class PermissionSetModelBuilder : IPermissionSetModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private RoleManager<IdentityRole> _roleManager;

        public PermissionSetModelBuilder(AtlasDbContext dbContext, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
        }

        public async Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId)
        {
            var result = new IndexPageModel();

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new IndexPageModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name,
                    IsInUse = true
                });
            }

            return result;
        }

        public async Task<FormComponentModel> BuildCreateFormModelAsync(Guid siteId)
        {
            var result = new FormComponentModel();

            foreach (var roleModel in await GetRoleModels())
            {
                var permissionModel = new FormComponentModel.PermissionModel
                {
                    RoleId = roleModel.Id,
                    RoleName = roleModel.Name,
                    Disabled = roleModel.Name == "Admin"
                };

                foreach (PermissionType permissionType in Enum.GetValues(typeof(PermissionType)))
                {
                    permissionModel.PermissionTypes.Add(new FormComponentModel.PermissionTypeModel
                    {
                        Type = permissionType,
                        Selected = roleModel.Name == "Admin"
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

            foreach (var roleModel in await GetRoleModels())
            {
                var permissionModel = new FormComponentModel.PermissionModel
                {
                    RoleId = roleModel.Id,
                    RoleName = roleModel.Name,
                    Disabled = roleModel.Name == Consts.RoleNameAdmin
                };

                foreach (PermissionType permissionType in Enum.GetValues(typeof(PermissionType)))
                {
                    var selected = permissionSet.Permissions
                                       .FirstOrDefault(x => x.Type == permissionType && 
                                                            x.RoleId == roleModel.Id) != null 
                                   || roleModel.Name == Consts.RoleNameAdmin;

                    permissionModel.PermissionTypes.Add(new FormComponentModel.PermissionTypeModel
                    {
                        Type = permissionType,
                        Selected = selected
                    });
                }

                result.PermissionSet.Permissions.Add(permissionModel);
            }

            return result;
        }

        private async Task<IList<RoleModel>> GetRoleModels()
        {
            var result = new List<RoleModel>
            {
                new RoleModel {Id = Consts.RoleIdAll, Name = "All Users"},
                new RoleModel {Id = Consts.RoleIdAuthorized, Name = "Registered Users"},
                new RoleModel {Id = Consts.RoleIdAnonymous, Name = "Anonymous Users"}
            };

            result.AddRange(from role in await _roleManager.Roles.ToListAsync() select new RoleModel {Id = role.Id, Name = role.Name});

            return result;
        }
    }

    public class RoleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
