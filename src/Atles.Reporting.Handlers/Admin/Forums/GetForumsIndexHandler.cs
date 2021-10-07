using Atles.Data;
using Atles.Domain.Categories;
using Atles.Domain.Forums;
using Atles.Infrastructure.Queries;
using Atles.Models.Admin.Forums;
using Atles.Reporting.Admin.Forums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Admin.Forums
{
    public class GetForumsIndexHandler : IQueryHandler<GetForumsIndex, IndexPageModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetForumsIndexHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IndexPageModel> Handle(GetForumsIndex query)
        {
            var categories = await _dbContext.Categories
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == query.SiteId && x.Status != CategoryStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            if (!categories.Any())
            {
                throw new ApplicationException("No Categories found.");
            }

            var currentCategory = query.CategoryId == null
                ? categories.FirstOrDefault()
                : categories.FirstOrDefault(x => x.Id == query.CategoryId);

            if (currentCategory == null)
            {
                throw new ApplicationException("Category not found.");
            }

            var forums = await _dbContext.Forums
                .Include(x => x.PermissionSet)
                .Where(x => x.CategoryId == currentCategory.Id && x.Status != ForumStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var result = new IndexPageModel();

            foreach (var category in categories)
            {
                result.Categories.Add(new IndexPageModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            foreach (var forum in forums)
            {
                var permissionSetName = !forum.HasPermissionSet()
                    ? $"{currentCategory.PermissionSetName()} (from Category)"
                    : forum.PermissionSetName();

                result.Forums.Add(new IndexPageModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    Slug = forum.Slug,
                    SortOrder = forum.SortOrder,
                    TotalTopics = forum.TopicsCount,
                    TotalReplies = forum.RepliesCount,
                    PermissionSetName = permissionSetName
                });
            }

            return result;
        }
    }
}
