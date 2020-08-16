using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Models;
using Atlas.Models.Public.Forums;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders.Public
{
    public class ForumModelBuilder : IForumModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IGravatarService _gravatarService;

        public ForumModelBuilder(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IGravatarService gravatarService)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _gravatarService = gravatarService;
        }

        public async Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, string slug, QueryOptions options)
        {
            var forum = await _dbContext.Forums
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.Slug == slug &&
                    x.Category.SiteId == siteId &&
                    x.Status == StatusType.Published);

            if (forum == null)
            {
                return null;
            }

            var result = new ForumPageModel
            {
                Forum = new ForumPageModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    Slug = forum.Slug
                },
                Topics = await BuildForumPageModelTopicsAsync(forum.Id, options)
            };

            return result;
        }

        public async Task<PaginatedData<ForumPageModel.TopicModel>> BuildForumPageModelTopicsAsync(Guid forumId, QueryOptions options)
        {
            var topicsQuery = _dbContext.Posts
                .Include(x => x.Member)
                .Include(x => x.LastReply).ThenInclude(x => x.Member)
                .Where(x =>
                    x.TopicId == null &&
                    x.ForumId == forumId &&
                    x.Status == StatusType.Published);

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                topicsQuery = topicsQuery
                    .Where(x => x.Title.Contains(options.Search) || x.Content.Contains(options.Search));
            }

            var topics = await topicsQuery
                .OrderByDescending(x => x.Pinned)
                    .ThenByDescending(x => x.LastReply != null ? x.LastReply.TimeStamp : x.TimeStamp)
                .Skip(options.Skip)
                .Take(options.PageSize)
                .ToListAsync();

            var items = topics.Select(topic => new ForumPageModel.TopicModel
            {
                Id = topic.Id,
                Title = topic.Title,
                Slug = topic.Slug,
                TotalReplies = topic.RepliesCount,
                MemberId = topic.Member.Id,
                MemberDisplayName = topic.Member.DisplayName,
                TimeStamp = topic.TimeStamp,
                GravatarHash = _gravatarService.HashEmailForGravatar(topic.Member.Email),
                MostRecentMemberId = topic.LastReply?.MemberId ?? topic.MemberId,
                MostRecentMemberDisplayName = topic.LastReply?.Member?.DisplayName ?? topic.Member.DisplayName,
                MostRecentTimeStamp = topic.LastReply?.TimeStamp ?? topic.TimeStamp,
                Pinned = topic.Pinned,
                Locked = topic.Locked
            })
            .ToList();

            var totalRecordsQuery = _dbContext.Posts
                .Where(x =>
                    x.TopicId == null &&
                    x.ForumId == forumId &&
                    x.Status == StatusType.Published);

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                totalRecordsQuery = totalRecordsQuery
                    .Where(x => x.Title.Contains(options.Search) || x.Content.Contains(options.Search));
            }

            var totalRecords = await totalRecordsQuery.CountAsync();

            var result = new PaginatedData<ForumPageModel.TopicModel>(items, totalRecords, options.PageSize);

            return result;
        }

    }
}