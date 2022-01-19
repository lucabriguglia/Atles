using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Models.Categories;
using Atles.Domain.Models.Forums;
using Atles.Domain.Models.PermissionSets;
using Atles.Infrastructure.Queries;
using Atles.Reporting.Models.Admin.Forums;
using Atles.Reporting.Models.Admin.Forums.Queries;
using Microsoft.EntityFrameworkCore;

namespace Atles.Reporting.Handlers.Admin
{
    public class GetForumEditFormHandler : IQueryHandler<GetForumEditForm, FormComponentModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetForumEditFormHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FormComponentModel> Handle(GetForumEditForm query)
        {
            var forum = await _dbContext.Forums
                .FirstOrDefaultAsync(x =>
                    x.Category.SiteId == query.SiteId &&
                    x.Id == query.Id &&
                    x.Status != ForumStatusType.Deleted);

            if (forum == null)
            {
                return null;
            }

            var result = new FormComponentModel
            {
                Forum = new FormComponentModel.ForumModel
                {
                    Id = forum.Id,
                    CategoryId = forum.CategoryId,
                    Name = forum.Name,
                    Slug = forum.Slug,
                    Description = forum.Description,
                    PermissionSetId = forum.PermissionSetId ?? Guid.Empty
                }
            };

            var categories = await _dbContext.Categories
                .Where(x => x.SiteId == query.SiteId && x.Status != CategoryStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var category in categories)
            {
                result.Categories.Add(new FormComponentModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == query.SiteId && x.Status == PermissionSetStatusType.Published)
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
