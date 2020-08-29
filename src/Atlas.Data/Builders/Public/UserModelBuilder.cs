using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Models;
using Atlas.Models.Public.Search;
using Atlas.Models.Public.Users;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders.Public
{
    public class UserModelBuilder : IUserModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IGravatarService _gravatarService;
        private readonly ISearchModelBuilder _searchModelBuilder;

        public UserModelBuilder(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IGravatarService gravatarService, 
            ISearchModelBuilder searchModelBuilder)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _gravatarService = gravatarService;
            _searchModelBuilder = searchModelBuilder;
        }

        public async Task<UserPageModel> BuildUserPageModelAsync(Guid userId, IList<Guid> forumIds)
        {
            var result = new UserPageModel();

            var member = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId);

            if (member == null)
            {
                return null;
            }

            result.User = new UserModel
            {
                Id = member.Id,
                DisplayName = member.DisplayName,
                TotalTopics = member.TopicsCount,
                TotalReplies = member.RepliesCount,
                GravatarHash = _gravatarService.HashEmailForGravatar(member.Email),
                Status = member.Status
            };

            result.Posts = await _searchModelBuilder.SearchPostModels(forumIds, new QueryOptions(), userId);

            return result;
        }

        public async Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid userId)
        {
            var result = new SettingsPageModel();

            var member = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId &&
                    x.Status != StatusType.Deleted);

            if (member == null)
            {
                return null;
            }

            result.User = new SettingsPageModel.UserModel
            {
                Id = member.Id,
                Email = member.Email,
                DisplayName = member.DisplayName,
                GravatarHash = _gravatarService.HashEmailForGravatar(member.Email),
                IsSuspended = member.Status == StatusType.Suspended
            };

            return result;
        }
    }
}