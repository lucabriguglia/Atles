using System;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain;
using Atles.Validators.Categories;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server.ValidationRules;

public class CategoryValidationRules : ICategoryValidationRules
{
    private readonly AtlesDbContext _dbContext;

    public CategoryValidationRules(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsCategoryNameUnique(Guid siteId, string name, Guid? id = null)
    {
        bool any;

        if (id != null)
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
}