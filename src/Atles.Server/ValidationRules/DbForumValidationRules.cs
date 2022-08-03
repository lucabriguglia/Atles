﻿using System;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain;
using Atles.Validators.ValidationRules;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server.ValidationRules;

public class DbForumValidationRules : IForumValidationRules
{
    private readonly AtlesDbContext _dbContext;

    public DbForumValidationRules(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsForumNameUnique(Guid siteId, Guid categoryId, string name, Guid? id = null)
    {
        bool any;

        if (id != null)
        {
            any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.CategoryId == categoryId &&
                               x.Name == name &&
                               x.Status != ForumStatusType.Deleted &&
                               x.Id != id);
        }
        else
        {
            any = any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.CategoryId == categoryId &&
                               x.Name == name &&
                               x.Status != ForumStatusType.Deleted);
        }

        return !any;
    }

    public async Task<bool> IsForumSlugUnique(Guid siteId, string slug, Guid? id = null)
    {
        bool any;

        if (id != null)
        {
            any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.Slug == slug &&
                                    x.Status != ForumStatusType.Deleted &&
                                    x.Id != id);
        }
        else
        {
            any = any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == siteId &&
                               x.Slug == slug &&
                               x.Status != ForumStatusType.Deleted);
        }

        return !any;
    }

    public async Task<bool> IsForumValid(Guid siteId, Guid id)
    {
        var any = await _dbContext.Forums
            .AnyAsync(x => x.Category.SiteId == siteId &&
                           x.Id == id &&
                           x.Status == ForumStatusType.Published);
        return any;
    }
}