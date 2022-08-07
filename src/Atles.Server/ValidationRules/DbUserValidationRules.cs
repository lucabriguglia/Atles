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

    public async Task<bool> IsUserEmailUnique(Guid id, string email)
    {
        bool any;

        if (id != Guid.Empty)
        {
            any = await _dbContext.Users
                .AnyAsync(x => x.Email == email &&
                               x.Status != UserStatusType.Deleted &&
                               x.Id != id);
        }
        else
        {
            any = await _dbContext.Users
                .AnyAsync(x => x.Email == email &&
                               x.Status != UserStatusType.Deleted);
        }

        return !any;
    }

    public async Task<bool> IsUserDisplayNameUnique(Guid id, string displayName)
    {
        bool any;

        if (id != Guid.Empty)
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
