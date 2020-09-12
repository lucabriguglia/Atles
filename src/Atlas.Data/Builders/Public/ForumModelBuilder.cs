using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.Forums;
using Atlas.Domain.Posts;
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
                    x.Status == ForumStatusType.Published);

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
                    Description = forum.Description,
                    Slug = forum.Slug
                },
                Topics = await BuildForumPageModelTopicsAsync(forum.Id, options)
            };

            return result;
        }

        public async Task<PaginatedData<ForumPageModel.TopicModel>> BuildForumPageModelTopicsAsync(Guid forumId, QueryOptions options)
        {
            var topicsQuery = _dbContext.Posts
                .Include(x => x.CreatedByUser)
                .Include(x => x.LastReply).ThenInclude(x => x.CreatedByUser)
                .Where(x =>
                    x.TopicId == null &&
                    x.ForumId == forumId &&
                    x.Status == PostStatusType.Published);

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                topicsQuery = topicsQuery
                    .Where(x => x.Title.Contains(options.Search) || x.Content.Contains(options.Search));
            }

            var topics = await topicsQuery
                .OrderByDescending(x => x.Pinned)
                    .ThenByDescending(x => x.LastReply != null ? x.LastReply.CreatedOn : x.CreatedOn)
                .Skip(options.Skip)
                .Take(options.PageSize)
                .ToListAsync();

            var items = topics.Select(topic => new ForumPageModel.TopicModel
            {
                Id = topic.Id,
                Title = topic.Title,
                Slug = topic.Slug,
                TotalReplies = topic.RepliesCount,
                UserId = topic.CreatedByUser.Id,
                UserDisplayName = topic.CreatedByUser.DisplayName,
                TimeStamp = topic.CreatedOn,
                GravatarHash = _gravatarService.HashEmailForGravatar(topic.CreatedByUser.Email),
                MostRecentUserId = topic.LastReply?.CreatedBy ?? topic.CreatedBy,
                MostRecentUserDisplayName = topic.LastReply?.CreatedByUser?.DisplayName ?? topic.CreatedByUser.DisplayName,
                MostRecentTimeStamp = topic.LastReply?.CreatedOn ?? topic.CreatedOn,
                Pinned = topic.Pinned,
                Locked = topic.Locked,
                HasAnswer = topic.HasAnswer
            })
            .ToList();

            var totalRecordsQuery = _dbContext.Posts
                .Where(x =>
                    x.TopicId == null &&
                    x.ForumId == forumId &&
                    x.Status == PostStatusType.Published);

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