using System;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain;
using Atles.Validators.ValidationRules;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server.ValidationRules;

public class DbUserValidationRules : IUserValidationRules
{
    private readonly AtlesDbContext _dbContext;

    public DbUserValidationRules(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsUserDisplayNameUnique(string displayName, Guid? id = null)
    {
        bool any;

        if (id != null)
        {
            any = await _dbContext.Users
                .AnyAsync(x => x.DisplayName == displayName &&
                               x.Status != UserStatusType.Deleted &&
                               x.Id != id);
        }
        else
        {
            any = await _dbContext.Users
                .AnyAsync(x => x.DisplayName == displayName &&
                               x.Status != UserStatusType.Deleted);
        }

        return !any;
    }
}
