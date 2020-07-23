using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Shared.Site;
using Atlas.Shared.Site.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Data.Builders.Site
{
    public class SiteModelBuilder : ISiteModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public SiteModelBuilder(AtlasDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId)
        {
            var model = new IndexPageModel();

            model.ForumGroups = await _cacheManager.GetOrSetAsync(CacheKeys.ForumGroups(siteId), async () =>
            {
                var forumGroups = await _dbContext.ForumGroups
                    .Include(x => x.PermissionSet)
                    .Where(x => x.SiteId == siteId && x.Status == StatusType.Published)
                    .OrderBy(x => x.SortOrder)
                    .ToListAsync();

                return forumGroups.Select(forumGroup => new IndexPageModel.ForumGroupModel
                {
                    Id = forumGroup.Id,
                    Name = forumGroup.Name
                }).ToList();
            });

            foreach (var forumGroup in model.ForumGroups)
            {
                forumGroup.Forums = await _cacheManager.GetOrSetAsync(CacheKeys.Forums(forumGroup.Id), async () =>
                {
                    var forums = await _dbContext.Forums
                        .Include(x => x.PermissionSet)
                        .Where(x => x.ForumGroupId == forumGroup.Id && x.Status == StatusType.Published)
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

            return result;
        }
    }
}
