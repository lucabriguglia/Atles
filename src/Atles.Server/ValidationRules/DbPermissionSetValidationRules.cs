using Atles.Data;
using Atles.Domain;
using Atles.Validators.ValidationRules;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server.ValidationRules;

public class DbPermissionSetValidationRules : IPermissionSetValidationRules
{
    private readonly AtlesDbContext _dbContext;

    public DbPermissionSetValidationRules(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsPermissionSetNameUnique(Guid siteId, Guid id, string name)
    {
        bool any;

        if (id != Guid.Empty)
        {
            any = await _dbContext.PermissionSets
                .AnyAsync(x => x.SiteId == siteId &&
                               x.Name == name &&
                               x.Status != PermissionSetStatusType.Deleted &&
                               x.Id != id);
        }
        else
        {
            any = await _dbContext.PermissionSets
                .AnyAsync(x => x.SiteId == siteId &&
                               x.Name == name &&
                               x.Status != PermissionSetStatusType.Deleted);
        }

        return !any;
    }

    public async Task<bool> IsPermissionSetValid(Guid siteId, Guid id)
    {
        var any = await _dbContext.PermissionSets
            .AnyAsync(x => x.SiteId == siteId &&
                           x.Id == id &&
                           x.Status == PermissionSetStatusType.Published);
        return any;
    }
}