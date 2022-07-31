using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Public;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public
{
    public class GetIndexPageHandler : IQueryHandler<GetIndexPage, IndexPageModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public GetIndexPageHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<QueryResult<IndexPageModel>> Handle(GetIndexPage query)
        {
            var model = new IndexPageModel
            {
                Categories = await _cacheManager.GetOrSetAsync(CacheKeys.Categories(query.SiteId), async () =>
                {
                    var categories = await _dbContext.Categories
                        .Include(x => x.Forums)
                        .Where(x => x.SiteId == query.SiteId && x.Status == CategoryStatusType.Published)
                        .OrderBy(x => x.SortOrder)
                        .ToListAsync();

                    return categories.Select(category => new IndexPageModel.CategoryModel
                    {
                        Id = category.Id,
                        Name = category.Name,
                        PermissionSetId = category.PermissionSetId,
                        ForumIds = category.Forums
                            .Where(x => x.Status == ForumStatusType.Published)
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
                            .Include(x => x.LastPost).ThenInclude(x => x.CreatedByUser)
                            .Include(x => x.LastPost).ThenInclude(x => x.Topic)
                            .Where(x => x.Id == forumId && x.Status == ForumStatusType.Published)
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
                                LastPostTimeStamp = entity.LastPost?.CreatedOn,
                                LastPostUserId = entity.LastPost?.CreatedByUser?.Id,
                                LastPostUserDisplayName = entity.LastPost?.CreatedByUser?.DisplayName
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
