using Atles.Data;
using Atles.Domain.Posts.Generators;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.Posts.Generators
{
    public class GetTopicSlugHandler : IQueryHandler<GetTopicSlug, string>
    {
        private readonly AtlesDbContext _dbContext;

        public GetTopicSlugHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> Handle(GetTopicSlug query)
        {
            var topic = await _dbContext.Posts.FirstOrDefaultAsync(x => x.Id == query.TopicId);

            return topic?.Slug;
        }
    }
}
