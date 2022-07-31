using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Queries.Public;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public
{
    public class GetTopicSlugHandler : IQueryHandler<GetTopicSlug, string>
    {
        private readonly AtlesDbContext _dbContext;

        public GetTopicSlugHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<string>> Handle(GetTopicSlug query)
        {
            var topic = await _dbContext.Posts.FirstOrDefaultAsync(x => x.Id == query.TopicId);

            return topic?.Slug;
        }
    }
}
