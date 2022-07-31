using System.Linq;
using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Models.Public;
using Atles.Queries.Public;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public
{
    public class GetUserTopicReactionsHandler : IQueryHandler<GetUserTopicReactions, UserTopicReactionsModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetUserTopicReactionsHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<UserTopicReactionsModel>> Handle(GetUserTopicReactions query)
        {
            var user = await _dbContext.Users
                .Include(x => x.PostReactions.Where(y => y.Post.Id == query.TopicId || y.Post.TopicId == query.TopicId))
                .FirstOrDefaultAsync(x => x.Id == query.UserId);

            if (user == null)
            {
                return null;
            }

            var result = new UserTopicReactionsModel();

            foreach (var postReaction in user.PostReactions)
            {
                result.PostReactions.Add(postReaction.PostId, postReaction.Type);
            }

            return result;
        }
    }
}
