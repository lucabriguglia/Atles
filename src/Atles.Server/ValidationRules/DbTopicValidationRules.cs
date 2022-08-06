using Atles.Data;
using Atles.Domain;
using Atles.Validators.ValidationRules;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server.ValidationRules;

public class DbTopicValidationRules : ITopicValidationRules
{
    private readonly AtlesDbContext _dbContext;

    public DbTopicValidationRules(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsTopicValid(Guid siteId, Guid forumId, Guid id)
    {
        return await _dbContext.Posts
            .AnyAsync(x =>
                x.ForumId == forumId &&
                x.Forum.Category.SiteId == siteId &&
                x.Id == id &&
                x.TopicId == null &&
                x.Status == PostStatusType.Published);
    }
}
