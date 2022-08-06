using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Admin.Categories;
using Atles.Queries.Admin;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Admin;

public class GetCategoryFormHandler : IQueryHandler<GetCategoryForm, CategoryFormModel>
{
    private readonly AtlesDbContext _dbContext;

    public GetCategoryFormHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QueryResult<CategoryFormModel>> Handle(GetCategoryForm query)
    {
        var result = new CategoryFormModel();

        var permissionSets = await _dbContext.PermissionSets
            .Where(x => x.SiteId == query.SiteId && x.Status == PermissionSetStatusType.Published)
            .ToListAsync();

        if (query.Id != null)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(x =>
                    x.SiteId == query.SiteId &&
                    x.Id == query.Id &&
                    x.Status != CategoryStatusType.Deleted);

            if (category == null)
            {
                return new Failure(FailureType.NotFound, "Category", $"Category with id {query.Id} not found.");
            }

            result.Category = new CategoryFormModel.CategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                PermissionSetId = category.PermissionSetId
            };
        }
        else
        {
            result.Category = new CategoryFormModel.CategoryModel
            {
                PermissionSetId = permissionSets.FirstOrDefault()?.Id ?? Guid.Empty
            };
        }

        foreach (var permissionSet in permissionSets)
        {
            result.PermissionSets.Add(new CategoryFormModel.PermissionSetModel
            {
                Id = permissionSet.Id,
                Name = permissionSet.Name
            });
        }

        return result;
    }
}
