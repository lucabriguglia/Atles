using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Shared.Public;
using Atlas.Shared.Public.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders.Public
{
    public class PublicModelBuilder : IPublicModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public PublicModelBuilder(AtlasDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId)
        {
            var model = new IndexPageModel
            {
                Categories = await _cacheManager.GetOrSetAsync(CacheKeys.Categories(siteId), async () =>
                {
                    var categories = await _dbContext.Categories
                        .Include(x => x.PermissionSet)
                        .Where(x => x.SiteId == siteId && x.Status == StatusType.Published)
                        .OrderBy(x => x.SortOrder)
                        .ToListAsync();

                    return categories.Select(category => new IndexPageModel.CategoryModel
                    {
                        Id = category.Id, Name = category.Name
                    }).ToList();
                })
            };

            foreach (var category in model.Categories)
            {
                category.Forums = await _cacheManager.GetOrSetAsync(CacheKeys.Forums(category.Id), async () =>
                {
                    var forums = await _dbContext.Forums
                        .Include(x => x.PermissionSet)
                        .Where(x => x.CategoryId == category.Id && x.Status == StatusType.Published)
                        .OrderBy(x => x.SortOrder)
                        .ToListAsync();

                    return forums.Select(forum => new IndexPageModel.ForumModel
                    {
                        Id = forum.Id,
                        Name = forum.Name,
                        TotalTopics = forum.TopicsCount,
                        TotalReplies = forum.RepliesCount
                    }).ToList();
                });
            }

            return model;
        }

        public async Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, Guid forumId)
        {
            var forum = await _dbContext.Forums
                .FirstOrDefaultAsync(x =>
                    x.Id == forumId &&
                    x.Status != StatusType.Deleted);

            if (forum == null)
            {
                return null;
            }

            var result = new ForumPageModel
            {
                Forum = new ForumPageModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name
                }
            };

            var topics = await _dbContext.Topics
                .Include(x => x.Member)
                .Where(x => 
                    x.ForumId == forum.Id && 
                    x.Status == StatusType.Published)
                .ToListAsync();

            foreach (var topic in topics)
            {
                result.Topics.Add(new ForumPageModel.TopicModel
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    TotalReplies = topic.RepliesCount,
                    MemberId = topic.Member.Id,
                    MemberDisplayName = topic.Member.DisplayName
                });
            }

            return result;
        }

        public async Task<TopicPageModel> BuildNewTopicPageModelAsync(Guid siteId, Guid forumId)
        {
            var forum = await _dbContext.Forums
                .FirstOrDefaultAsync(x =>
                    x.Id == forumId &&
                    x.Status != StatusType.Deleted);

            if (forum == null)
            {
                return null;
            }

            var result = new TopicPageModel
            {
                Forum = new TopicPageModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name
                }
            };

            return result;
        }
    }
}
