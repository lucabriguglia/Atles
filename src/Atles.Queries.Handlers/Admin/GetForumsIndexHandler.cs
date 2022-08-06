using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Admin.Forums;
using Atles.Queries.Admin;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Admin;

public class GetForumsIndexHandler : IQueryHandler<GetForumsIndex, ForumsPageModel>
{
    private readonly AtlesDbContext _dbContext;

    public GetForumsIndexHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QueryResult<ForumsPageModel>> Handle(GetForumsIndex query)
    {
        var categories = await _dbContext.Categories
            .Include(x => x.PermissionSet)
            .Where(x => x.SiteId == query.SiteId && x.Status != CategoryStatusType.Deleted)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        if (!categories.Any())
        {
            return new Failure(FailureType.Error, "Categories", "No categories found.");
        }

        var currentCategory = query.CategoryId == null
            ? categories.FirstOrDefault()
            : categories.FirstOrDefault(x => x.Id == query.CategoryId);

        if (currentCategory == null)
        {
            return new Failure(FailureType.NotFound, "Category", $"Category with id {query.CategoryId} not found.");
        }

        var forums = await _dbContext.Forums
            .Include(x => x.PermissionSet)
            .Where(x => x.CategoryId == currentCategory.Id && x.Status != ForumStatusType.Deleted)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        var result = new ForumsPageModel();

        foreach (var category in categories)
        {
            result.Categories.Add(new ForumsPageModel.CategoryModel
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

            result.Forums.Add(new ForumsPageModel.ForumModel
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
