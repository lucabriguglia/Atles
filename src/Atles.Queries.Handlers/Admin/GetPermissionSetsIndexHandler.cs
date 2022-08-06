using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Admin.PermissionSets;
using Atles.Queries.Admin;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Admin;

public class GetPermissionSetsIndexHandler : IQueryHandler<GetPermissionSetsIndex, PermissionSetsPageModel>
{
    private readonly AtlesDbContext _dbContext;

    public GetPermissionSetsIndexHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QueryResult<PermissionSetsPageModel>> Handle(GetPermissionSetsIndex query)
    {
        var result = new PermissionSetsPageModel();

        var permissionSets = await _dbContext.PermissionSets
            .Include(x => x.Categories)
            .Include(x => x.Forums)
            .Where(x => x.SiteId == query.SiteId && x.Status != PermissionSetStatusType.Deleted)
            .ToListAsync();

        foreach (var permissionSet in permissionSets)
        {
            result.PermissionSets.Add(new PermissionSetsPageModel.PermissionSetModel
            {
                Id = permissionSet.Id,
                Name = permissionSet.Name,
                IsInUse = permissionSet.Categories.Any() || permissionSet.Forums.Any()
            });
        }

        return result;
    }
}
