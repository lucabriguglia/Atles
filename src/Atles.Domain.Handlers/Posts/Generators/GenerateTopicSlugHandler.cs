using Atles.Data;
using Atles.Domain.Posts;
using Atles.Domain.Posts.Generators;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;
using System;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.Posts.Generators
{
    public class GenerateTopicSlugHandler : IQueryHandler<GenerateTopicSlug, string>
    {
        private readonly AtlesDbContext _dbContext;

        public GenerateTopicSlugHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> Handle(GenerateTopicSlug query)
        {
            var slug = string.Empty;
            var exists = true;
            var repeat = 0;

            while (exists && repeat < 5)
            {
                var suffix = repeat > 0 ? $"-{repeat}" : string.Empty;
                slug = $"{query.Title.ToSlug()}{suffix}";
                exists = await _dbContext.Posts.AnyAsync(x =>
                    x.ForumId == query.ForumId &&
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
