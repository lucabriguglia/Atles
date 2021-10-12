using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Data.Caching;
using Atles.Domain.Users;
using Atles.Models;
using Atles.Models.Public.Users;
using Atles.Reporting.Public.Queries;
using Microsoft.EntityFrameworkCore;
using OpenCqrs;

namespace Atles.Data.Builders.Public
{
    public class UserModelBuilder : IUserModelBuilder
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IGravatarService _gravatarService;
        private readonly ISender _sender;

        public UserModelBuilder(AtlesDbContext dbContext,
            ICacheManager cacheManager,
            IGravatarService gravatarService, 
            ISender sender)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _gravatarService = gravatarService;
            _sender = sender;
        }

        public async Task<UserPageModel> BuildUserPageModelAsync(Guid userId, IList<Guid> forumIds)
        {
            var result = new UserPageModel();

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId);

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
                GravatarHash = _gravatarService.HashEmailForGravatar(user.Email),
                Status = user.Status
            };

            result.Posts = await _sender.Send(new GetSearchPosts { AccessibleForumIds = forumIds, Options = new QueryOptions(), UserId = userId });

            return result;
        }

        public async Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid userId)
        {
            var result = new SettingsPageModel();

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId &&
                    x.Status != UserStatusType.Deleted);

            if (user == null)
            {
                return null;
            }

            result.User = new SettingsPageModel.UserModel
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                GravatarHash = _gravatarService.HashEmailForGravatar(user.Email),
                IsSuspended = user.Status == UserStatusType.Suspended
            };

            return result;
        }
    }
}