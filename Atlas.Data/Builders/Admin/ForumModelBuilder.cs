using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Shared.Admin.Forums;
using Atlas.Shared.Admin.Forums.Models;

namespace Atlas.Data.Builders.Admin
{
    public class ForumModelBuilder : IForumModelBuilder
    {
        private readonly AtlasDbContext _dbContext;

        public ForumModelBuilder(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IndexModel> BuildIndexModelAsync(Guid siteId, Guid? categoryId = null)
        {
            var categories = await _dbContext.Categories
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            if (!categories.Any())
            {
                throw new ApplicationException("No Categories found.");
            }

            var currentCategory = categoryId == null
                ? categories.FirstOrDefault()
                : categories.FirstOrDefault(x => x.Id == categoryId);

            if (currentCategory == null)
            {
                throw new ApplicationException("Category not found.");
            }

            var forums = await _dbContext.Forums
                .Include(x => x.PermissionSet)
                .Where(x => x.CategoryId == currentCategory.Id && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var result = new IndexModel();

            foreach (var category in categories)
            {
                result.Categories.Add(new IndexModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            foreach (var forum in forums)
            {
                var permissionSetName = !forum.HasPermissionSet()
                    ? !currentCategory.HasPermissionSet()
                        ? "Default (from Site)"
                        : $"{currentCategory.PermissionSetName()} (from Category)"
                    : forum.PermissionSetName();

                result.Forums.Add(new IndexModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    SortOrder = forum.SortOrder,
                    TotalTopics = forum.TopicsCount,
                    TotalReplies = forum.RepliesCount,
                    PermissionSetName = permissionSetName
                });
            }

            return result;
        }

        public async Task<FormModel> BuildCreateFormModelAsync(Guid siteId, Guid? categoryId = null)
        {
            var result = new FormModel();

            var categories = await _dbContext.Categories
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var category in categories)
            {
                result.Categories.Add(new FormModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            var selectedCategoryId = categoryId == null 
                ? categories.FirstOrDefault().Id 
                : categoryId.Value;

            result.Forum = new FormModel.ForumModel
            {
                CategoryId = selectedCategoryId
            };

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new FormModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name
                });
            }

            return result;
        }

        public async Task<FormModel> BuildEditFormModelAsync(Guid siteId, Guid id)
        {
            var forum = await _dbContext.Forums
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.Status != StatusType.Deleted);

            if (forum == null)
            {
                return null;
            }

            var result = new FormModel
            {
                Forum = new FormModel.ForumModel
                {
                    Id = forum.Id,
                    CategoryId = forum.CategoryId,
                    Name = forum.Name,
                    PermissionSetId = forum.PermissionSetId ?? Guid.Empty
                }
            };

            var categories = await _dbContext.Categories
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var category in categories)
            {
                result.Categories.Add(new FormModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == siteId && x.Status != StatusType.Deleted)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new FormModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name
                });
            }

            return result;
        }
    }
}
