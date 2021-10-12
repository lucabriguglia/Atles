using Atles.Data;
using Atles.Models;
using Atles.Models.Public.Users;
using Atles.Reporting.Public.Queries;
using Atles.Reporting.Shared.Queries;
using Microsoft.EntityFrameworkCore;
using OpenCqrs;
using OpenCqrs.Queries;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Public
{
    public class GetUserPageHandler : IQueryHandler<GetUserPage, UserPageModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ISender _sender;

        public GetUserPageHandler(AtlesDbContext dbContext, ISender sender)
        {
            _dbContext = dbContext;
            _sender = sender;
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
                GravatarHash = _sender.Send(new GenerateEmailHashForGravatar { Email = user.Email }).GetAwaiter().GetResult(),
                Status = user.Status
            };

            result.Posts = await _sender.Send(new GetSearchPosts 
            { 
                AccessibleForumIds = query.AccessibleForumIds, 
                Options = new QueryOptions(), 
                UserId = query.UserId 
            });

            return result;
        }
    }
}
