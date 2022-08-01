using System;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain;
using Atles.Validators.PermissionSets;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server.ValidationRules;

public class DbPermissionSetValidationRules : IPermissionSetValidationRules
{
    private readonly AtlesDbContext _dbContext;

    public DbPermissionSetValidationRules(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
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