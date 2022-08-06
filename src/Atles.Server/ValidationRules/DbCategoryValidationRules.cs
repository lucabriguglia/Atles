using Atles.Data;
using Atles.Domain;
using Atles.Validators.ValidationRules;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server.ValidationRules;

public class DbCategoryValidationRules : ICategoryValidationRules
{
    private readonly AtlesDbContext _dbContext;

    public DbCategoryValidationRules(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsCategoryNameUnique(Guid siteId, string name, Guid? id = null)
    {
        bool any;

        if (id != null && id != Guid.Empty)
        {
            any = await _dbContext.Categories
                .AnyAsync(x => x.SiteId == siteId &&
                               x.Name == name &&
                               x.Status != CategoryStatusType.Deleted &&
                               x.Id != id);
        }
        else
        {
            any = await _dbContext.Categories
                .AnyAsync(x => x.SiteId == siteId &&
                               x.Name == name &&
                               x.Status != CategoryStatusType.Deleted);
        }

        return !any;
    }

    public async Task<bool> IsCategoryValid(Guid siteId, Guid id)
    {
        return await _dbContext.Categories
            .AnyAsync(x =>
                x.SiteId == siteId &&
                x.Id == id &&
                x.Status != CategoryStatusType.Deleted);
    }
}