using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.PermissionSets;
using Atlas.Models;
using Atlas.Models.Public;
using Markdig;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders.Public
{
    public class PublicModelBuilder : IPublicModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IRoleModelBuilder _roles;

        public PublicModelBuilder(AtlasDbContext dbContext, ICacheManager cacheManager, IRoleModelBuilder roles)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _roles = roles;
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
                        Description = forum.Description,
                        TotalTopics = forum.TopicsCount,
                        TotalReplies = forum.RepliesCount
                    }).ToList();
                });
            }

            return model;
        }

        public async Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, Guid forumId, PaginationOptions options)
        {
            var forum = await _dbContext.Forums
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.Id == forumId &&
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
                    Name = forum.Name
                },
                Permissions = await BuildPermissionModels(siteId, forum.PermissionSetId ?? forum.Category.PermissionSetId)
            };

            var topics = await _dbContext.Topics
                .Include(x => x.Member)
                .Where(x => 
                    x.ForumId == forum.Id && 
                    x.Status == StatusType.Published)
                .OrderByDescending(x => x.TimeStamp)
                .Skip(options.Skip)
                .Take(options.PageSize)
                .ToListAsync();

            var items = topics.Select(topic => new ForumPageModel.TopicModel
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    TotalReplies = topic.RepliesCount,
                    MemberId = topic.Member.Id,
                    MemberDisplayName = topic.Member.DisplayName,
                    TimeStamp = topic.TimeStamp
                })
                .ToList();

            var totalRecords = await _dbContext.Topics
                .Where(x =>
                    x.ForumId == forum.Id &&
                    x.Status == StatusType.Published)
                .CountAsync();

            result.Topics = new PaginatedData<ForumPageModel.TopicModel>(items, totalRecords, options.PageSize);

            return result;
        }

        public async Task<PostPageModel> BuildPostPageModelAsync(Guid siteId, Guid forumId)
        {
            var forum = await _dbContext.Forums
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.Id == forumId &&
                    x.Category.SiteId == siteId &&
                    x.Status == StatusType.Published);

            if (forum == null)
            {
                return null;
            }

            var result = new PostPageModel
            {
                Forum = new PostPageModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name
                },
                Permissions = await BuildPermissionModels(siteId, forum.PermissionSetId ?? forum.Category.PermissionSetId)
            };

            return result;
        }

        public async Task<TopicPageModel> BuildTopicPageModelAsync(Guid siteId, Guid forumId, Guid topicId, PaginationOptions options)
        {
            var topic = await _dbContext.Topics
                .Include(x => x.Forum).ThenInclude(x => x.Category)
                .Include(x => x.Member)
                .FirstOrDefaultAsync(x =>
                    x.Forum.Category.SiteId == siteId &&
                    x.Forum.Id == forumId &&
                    x.Id == topicId &&
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
                    Name = topic.Forum.Name
                },
                Topic = new TopicPageModel.TopicModel
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    Content = Markdown.ToHtml(topic.Content),
                    MemberId = topic.Member.Id,
                    MemberDisplayName = topic.Member.DisplayName,
                    TimeStamp = topic.TimeStamp
                },
                Permissions = await BuildPermissionModels(siteId, topic.Forum.PermissionSetId ?? topic.Forum.Category.PermissionSetId)
            };

            var replies = await _dbContext.Replies
                .Where(x => 
                    x.TopicId == topicId && 
                    x.Status == StatusType.Published)
                .OrderBy(x => x.TimeStamp)
                .Skip(options.Skip)
                .Take(options.PageSize)
                .ToListAsync();

            var items = replies.Select(reply => new TopicPageModel.ReplyModel
                {
                    Id = reply.Id,
                    Content = Markdown.ToHtml(reply.Content),
                    MemberId = reply.Member.Id,
                    MemberDisplayName = reply.Member.DisplayName,
                    TimeStamp = reply.TimeStamp
                }).ToList();

            var totalRecords = await _dbContext.Replies
                .Where(x =>
                    x.TopicId == topic.Id &&
                    x.Status == StatusType.Published)
                .CountAsync();

            result.Replies = new PaginatedData<TopicPageModel.ReplyModel>(items, totalRecords, options.PageSize);

            return result;
        }

        private async Task<IList<PermissionModel>> BuildPermissionModels(Guid siteId, Guid permissionSetId)
        {
            return await _cacheManager.GetOrSetAsync(CacheKeys.PermissionSet(permissionSetId), async () =>
            {
                var result = new List<PermissionModel>();

                var permissionSet = await _dbContext.PermissionSets
                    .Include(x => x.Permissions)
                    .FirstOrDefaultAsync(x =>
                        x.SiteId == siteId &&
                        x.Id == permissionSetId &&
                        x.Status != StatusType.Deleted);

                if (permissionSet == null)
                {
                    return result;
                }

                var roles = await _roles.GetRoleModels();

                foreach (PermissionType permissionType in Enum.GetValues(typeof(PermissionType)))
                {
                    var permissionModel = new PermissionModel
                    {
                        Type = permissionType
                    };

                    var permissions = permissionSet.Permissions.Where(x => x.Type == permissionType);

                    permissionModel.AllUsers = permissions.FirstOrDefault(x => x.RoleId == Consts.RoleIdAll) != null;
                    permissionModel.RegisteredUsers = permissions.FirstOrDefault(x => x.RoleId == Consts.RoleIdRegistered) != null;

                    foreach (var permission in permissions)
                    {
                        var role = roles.FirstOrDefault(x => x.Id == permission.RoleId);

                        if (role != null)
                        {
                            permissionModel.Roles.Add(new RoleModel
                            {
                                Id = role.Id,
                                Name = role.Name
                            });
                        }
                    }

                    result.Add(permissionModel);
                }

                return result;
            });
        }
    }
}
