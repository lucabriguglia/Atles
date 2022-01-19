using System.Threading.Tasks;
using Atles.Data;
using Atles.Infrastructure.Queries;
using Atles.Reporting.Models.Public.Queries;
using Microsoft.EntityFrameworkCore;

namespace Atles.Reporting.Handlers.Public
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
