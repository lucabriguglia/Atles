using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Models.Admin.Categories;

namespace Atlas.Data.Builders.Admin
{
    public class CategoryModelBuilder : ICategoryModelBuilder
    {
        private readonly AtlasDbContext _dbContext;

        public CategoryModelBuilder(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId)
        {
            var result = new IndexPageModel();

            var categories = await _dbContext.Categories
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var category in categories)
            {
                var forumsCount = await _dbContext.Forums
                    .Where(x => 
                        x.CategoryId == category.Id && 
                        x.Status != StatusType.Deleted)
                    .CountAsync();

                result.Categories.Add(new IndexPageModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    SortOrder = category.SortOrder,
                    TotalForums = forumsCount,
                    TotalTopics = category.TopicsCount,
                    TotalReplies = category.RepliesCount,
                    PermissionSetName = category.PermissionSetName()
                });
            }

            return result;
        }

        public async Task<FormComponentModel> BuildFormModelAsync(Guid siteId, Guid? id = null)
        {
            var result = new FormComponentModel();

            if (id != null)
            {
                var category = await _dbContext.Categories
                    .FirstOrDefaultAsync(x => 
                        x.SiteId == siteId && 
                        x.Id == id && 
                        x.Status != StatusType.Deleted);

                if (category == null)
                {
                    return null;
                }

                result.Category = new FormComponentModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    PermissionSetId = category.PermissionSetId
                };
            }

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == siteId && x.Status == StatusType.Published)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new FormComponentModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name
                });
            }

            return result;
        }
    }
}
