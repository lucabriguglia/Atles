using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Models.Admin.PermissionSets;

namespace Atlas.Data.Builders.Admin
{
    public class PermissionSetModelBuilder : IPermissionSetModelBuilder
    {
        private readonly AtlasDbContext _dbContext;

        public PermissionSetModelBuilder(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
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
                    Name = permissionSet.Name
                });
            }

            return result;
        }

        public async Task<FormComponentModel> BuildFormModelAsync(Guid siteId, Guid? id = null)
        {
            var result = new FormComponentModel();

            if (id != null)
            {
                var permissionSet = await _dbContext.PermissionSets
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
                    Name = permissionSet.Name,
                    Permissions = permissionSet.Permissions
                };
            }

            return result;
        }
    }
}
