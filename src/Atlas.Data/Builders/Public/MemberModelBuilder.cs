using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Models;
using Atlas.Models.Public.Members;
using Atlas.Models.Public.Search;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders.Public
{
    public class MemberModelBuilder : IMemberModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IGravatarService _gravatarService;
        private readonly ISearchModelBuilder _searchModelBuilder;

        public MemberModelBuilder(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IGravatarService gravatarService, 
            ISearchModelBuilder searchModelBuilder)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _gravatarService = gravatarService;
            _searchModelBuilder = searchModelBuilder;
        }

        public async Task<MemberPageModel> BuildMemberPageModelAsync(Guid memberId, IList<Guid> forumIds)
        {
            var result = new MemberPageModel();

            var member = await _dbContext.Members
                .FirstOrDefaultAsync(x =>
                    x.Id == memberId);

            if (member == null)
            {
                return null;
            }

            result.Member = new MemberModel
            {
                Id = member.Id,
                DisplayName = member.DisplayName,
                TotalTopics = member.TopicsCount,
                TotalReplies = member.RepliesCount,
                GravatarHash = _gravatarService.HashEmailForGravatar(member.Email),
                Status = member.Status
            };

            result.Posts = await _searchModelBuilder.SearchPostModels(forumIds, new QueryOptions(1), memberId);

            return result;
        }

        public async Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid memberId)
        {
            var result = new SettingsPageModel();

            var member = await _dbContext.Members
                .FirstOrDefaultAsync(x =>
                    x.Id == memberId &&
                    x.Status != StatusType.Deleted);

            if (member == null)
            {
                return null;
            }

            result.Member = new SettingsPageModel.MemberModel
            {
                Id = member.Id,
                Email = member.Email,
                DisplayName = member.DisplayName,
                GravatarHash = _gravatarService.HashEmailForGravatar(member.Email)
            };

            return result;
        }
    }
}