using Atles.Core.Extensions;
using Atles.Data;
using Atles.Domain;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Posts.Services
{
    public class TopicSlugGenerator : ITopicSlugGenerator
    {
        private readonly AtlesDbContext _dbContext;

        public TopicSlugGenerator(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GenerateTopicSlug(Guid forumId, string title)
        {
            var slug = string.Empty;
            var exists = true;
            var repeat = 0;

            while (exists && repeat < 5)
            {
                var suffix = repeat > 0 ? $"-{repeat}" : string.Empty;
                slug = $"{title.ToSlug()}{suffix}";
                exists = await _dbContext.Posts.AnyAsync(x =>
                    x.ForumId == forumId &&
                    x.Slug == slug &&
                    x.Status == PostStatusType.Published);
                repeat++;
            }

            if (exists)
            {
                slug = Guid.NewGuid().ToString();
            }

            return slug;
        }
    }
}
