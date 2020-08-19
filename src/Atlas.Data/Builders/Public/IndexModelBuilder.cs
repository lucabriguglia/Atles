using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Models.Public.Index;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders.Public
{
    public class IndexModelBuilder : IIndexModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IGravatarService _gravatarService;

        public IndexModelBuilder(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IGravatarService gravatarService)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _gravatarService = gravatarService;
        }

        public async Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId)
        {
            var model = new IndexPageModel
            {
                Categories = await _cacheManager.GetOrSetAsync(CacheKeys.Categories(siteId), async () =>
                {
                    var categories = await _dbContext.Categories
                        .Include(x => x.Forums)
                        .Where(x => x.SiteId == siteId && x.Status == StatusType.Published)
                        .OrderBy(x => x.SortOrder)
                        .ToListAsync();

                    return categories.Select(category => new IndexPageModel.CategoryModel
                    {
                        Id = category.Id,
                        Name = category.Name,
                        PermissionSetId = category.PermissionSetId,
                        ForumIds = category.Forums
                            .Where(x => x.Status == StatusType.Published)
                            .OrderBy(x => x.SortOrder)
                            .Select(x => x.Id)
                            .ToList()
                    }).ToList();
                })
            };

            foreach (var category in model.Categories)
            {
                category.Forums = new List<IndexPageModel.ForumModel>();

                foreach (var forumId in category.ForumIds)
                {
                    var forum = await _cacheManager.GetOrSetAsync(CacheKeys.Forum(forumId), async () =>
                    {
                        var entity = await _dbContext.Forums
                            .Include(x => x.LastPost).ThenInclude(x => x.CreatedByMember)
                            .Include(x => x.LastPost).ThenInclude(x => x.Topic)
                            .Where(x => x.Id == forumId && x.Status == StatusType.Published)
                            .OrderBy(x => x.SortOrder)
                            .FirstOrDefaultAsync();

                        if (entity != null)
                        {
                            return new IndexPageModel.ForumModel
                            {
                                Id = entity.Id,
                                Name = entity.Name,
                                Slug = entity.Slug,
                                Description = entity.Description,
                                TotalTopics = entity.TopicsCount,
                                TotalReplies = entity.RepliesCount,
                                PermissionSetId = entity.PermissionSetId,
                                LastTopicId = entity.LastPost?.TopicId == null ? entity.LastPost?.Id : entity.LastPost?.Topic?.Id,
                                LastTopicTitle = entity.LastPost?.Title ?? entity.LastPost?.Topic?.Title,
                                LastTopicSlug = entity.LastPost?.Slug ?? entity.LastPost?.Topic?.Slug,
                                LastPostTimeStamp = entity.LastPost?.TimeStamp,
                                LastPostMemberId = entity.LastPost?.CreatedByMember?.Id,
                                LastPostMemberDisplayName = entity.LastPost?.CreatedByMember?.DisplayName
                            };
                        }

                        return new IndexPageModel.ForumModel();
                    });

                    category.Forums.Add(forum);
                }
            }

            return model;
        }
    }
}