using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Admin.Forums;
using Atles.Queries.Admin;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Admin;

public class GetForumEditFormHandler : IQueryHandler<GetForumEditForm, ForumFormModel>
{
    private readonly AtlesDbContext _dbContext;

    public GetForumEditFormHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QueryResult<ForumFormModel>> Handle(GetForumEditForm query)
    {
        var forum = await _dbContext.Forums
            .FirstOrDefaultAsync(x =>
                x.Category.SiteId == query.SiteId &&
                x.Id == query.Id &&
                x.Status != ForumStatusType.Deleted);

        if (forum == null)
        {
            return new Failure(FailureType.NotFound, "Forum", $"Forum with id {query.Id} not found.");
        }

        var result = new ForumFormModel
        {
            Forum = new ForumFormModel.ForumModel
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
            result.Categories.Add(new ForumFormModel.CategoryModel
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
            result.PermissionSets.Add(new ForumFormModel.PermissionSetModel
            {
                Id = permissionSet.Id,
                Name = permissionSet.Name
            });
        }

        return result;
    }
}
