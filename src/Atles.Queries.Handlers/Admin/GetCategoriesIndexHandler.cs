using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Admin.Categories;
using Atles.Queries.Admin;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Admin;

public class GetCategoriesIndexHandler : IQueryHandler<GetCategoriesIndex, CategoriesPageModel>
{
    private readonly AtlesDbContext _dbContext;

    public GetCategoriesIndexHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QueryResult<CategoriesPageModel>> Handle(GetCategoriesIndex query)
    {
        var result = new CategoriesPageModel();

        var categories = await _dbContext.Categories
            .Include(x => x.PermissionSet)
            .Where(x => x.SiteId == query.SiteId && x.Status != CategoryStatusType.Deleted)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        foreach (var category in categories)
        {
            var forumsCount = await _dbContext.Forums
                .Where(x =>
                    x.CategoryId == category.Id &&
                    x.Status != ForumStatusType.Deleted)
                .CountAsync();

            result.Categories.Add(new CategoriesPageModel.CategoryModel
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
}
