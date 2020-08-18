using System;
using System.Linq;
using System.Threading.Tasks;
using Atlify.Models;
using Atlify.Models.Public.Topics;
using Atlify.Data.Caching;
using Atlify.Domain;
using Markdig;
using Microsoft.EntityFrameworkCore;

namespace Atlify.Data.Builders.Public
{
    public class TopicModelBuilder : ITopicModelBuilder
    {
        private readonly AtlifyDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IGravatarService _gravatarService;

        public TopicModelBuilder(AtlifyDbContext dbContext,
            ICacheManager cacheManager,
            IGravatarService gravatarService)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _gravatarService = gravatarService;
        }

        public async Task<TopicPageModel> BuildTopicPageModelAsync(Guid siteId, string forumSlug, string topicSlug, QueryOptions options)
        {
            var topic = await _dbContext.Posts
                .Include(x => x.Forum).ThenInclude(x => x.Category)
                .Include(x => x.Member)
                .FirstOrDefaultAsync(x =>
                    x.TopicId == null &&
                    x.Forum.Category.SiteId == siteId &&
                    x.Forum.Slug == forumSlug &&
                    x.Slug == topicSlug &&
                    x.Status == StatusType.Published);

            if (topic == null)
            {
                return null;
            }

            var result = new TopicPageModel
            {
                Forum = new TopicPageModel.ForumModel
                {
                    Id = topic.Forum.Id,
                    Name = topic.Forum.Name,
                    Slug = topic.Forum.Slug
                },
                Topic = new TopicPageModel.TopicModel
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    Slug = topic.Slug,
                    Content = Markdown.ToHtml(topic.Content),
                    MemberId = topic.Member.Id,
                    MemberDisplayName = topic.Member.DisplayName,
                    TimeStamp = topic.TimeStamp,
                    UserId = topic.Member.UserId,
                    GravatarHash = _gravatarService.HashEmailForGravatar(topic.Member.Email),
                    Pinned = topic.Pinned,
                    Locked = topic.Locked
                },
                Replies = await BuildTopicPageModelRepliesAsync(topic.Id, options)
            };

            return result;
        }

        public async Task<PaginatedData<TopicPageModel.ReplyModel>> BuildTopicPageModelRepliesAsync(Guid topicId, QueryOptions options)
        {
            var repliesQuery = _dbContext.Posts
                .Include(x => x.Member)
                .Where(x =>
                    x.TopicId == topicId &&
                    x.Status == StatusType.Published);

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                repliesQuery = repliesQuery.Where(x => x.Content.Contains(options.Search));
            }

            var replies = await repliesQuery
                .OrderBy(x => x.TimeStamp)
                .Skip(options.Skip)
                .Take(options.PageSize)
                .ToListAsync();

            var items = replies.Select(reply => new TopicPageModel.ReplyModel
            {
                Id = reply.Id,
                Content = Markdown.ToHtml(reply.Content),
                OriginalContent = reply.Content,
                UserId = reply.Member.UserId,
                MemberId = reply.Member.Id,
                MemberDisplayName = reply.Member.DisplayName,
                TimeStamp = reply.TimeStamp,
                GravatarHash = _gravatarService.HashEmailForGravatar(reply.Member.Email)
            }).ToList();

            var totalRecordsQuery = _dbContext.Posts
                .Where(x =>
                    x.TopicId == topicId &&
                    x.Status == StatusType.Published);

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                totalRecordsQuery = totalRecordsQuery.Where(x => x.Content.Contains(options.Search));
            }

            var totalRecords = await totalRecordsQuery.CountAsync();

            var result = new PaginatedData<TopicPageModel.ReplyModel>(items, totalRecords, options.PageSize);

            return result;
        }
    }
}