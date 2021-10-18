using Atles.Data;
using Atles.Models;
using Atles.Models.Public.Users;
using Atles.Reporting.Handlers.Services;
using Atles.Reporting.Public.Queries;
using Microsoft.EntityFrameworkCore;
using OpenCqrs;
using OpenCqrs.Queries;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Public
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

        public async Task<UserPageModel> Handle(GetUserPage query)
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

            result.Posts = await _dispatcher.Get(new GetSearchPosts 
            { 
                AccessibleForumIds = query.AccessibleForumIds, 
                Options = new QueryOptions(), 
                UserId = query.UserId 
            });

            return result;
        }
    }
}
