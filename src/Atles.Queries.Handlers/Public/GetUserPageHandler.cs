using System.Threading.Tasks;
using Atles.Core;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Models;
using Atles.Models.Public;
using Atles.Queries.Handlers.Services;
using Atles.Queries.Public;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public
{
    public class GetUserPageHandler : IQueryHandler<GetUserPage, UserPageModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IDispatcher _dispatcher;
        private readonly IGravatarService _gravatarService;

        public GetUserPageHandler(AtlesDbContext dbContext, IDispatcher sender, IGravatarService gravatarService)
        {
            _dbContext = dbContext;
            _dispatcher = sender;
            _gravatarService = gravatarService;
        }

        public async Task<QueryResult<UserPageModel>> Handle(GetUserPage query)
        {
            var result = new UserPageModel();

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == query.UserId);

            if (user == null)
            {
                return null;
            }

            result.User = new UserModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                TotalTopics = user.TopicsCount,
                TotalReplies = user.RepliesCount,
                GravatarHash = _gravatarService.GenerateEmailHash(user.Email),
                Status = user.Status
            };

            // TODO: To be moved to a service
            var queryResult = await _dispatcher.Get(new GetSearchPosts
            {
                AccessibleForumIds = query.AccessibleForumIds,
                Options = new QueryOptions(),
                UserId = query.UserId
            });
            var posts = queryResult.AsT0;

            result.Posts = posts;

            return result;
        }
    }
}
