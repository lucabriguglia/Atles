using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Models;
using Atlas.Models.Public;
using Atlas.Models.Public.Members;
using Markdig;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders.Public
{
    public class PublicModelBuilder : IPublicModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IGravatarService _gravatarService;

        public PublicModelBuilder(AtlasDbContext dbContext, 
            ICacheManager cacheManager, 
            IGravatarService gravatarService)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _gravatarService = gravatarService;
        }

        public async Task<IndexPageModelToFilter> BuildIndexPageModelToFilterAsync(Guid siteId)
        {
            var model = new IndexPageModelToFilter
            {
                Categories = await _cacheManager.GetOrSetAsync(CacheKeys.Categories(siteId), async () =>
                {
                    var categories = await _dbContext.Categories
                        .Where(x => x.SiteId == siteId && x.Status == StatusType.Published)
                        .OrderBy(x => x.SortOrder)
                        .ToListAsync();

                    return categories.Select(category => new IndexPageModelToFilter.CategoryModel
                    {
                        Id = category.Id, 
                        Name = category.Name,
                        PermissionSetId = category.PermissionSetId
                    }).ToList();
                })
            };

            foreach (var category in model.Categories)
            {
                category.Forums = await _cacheManager.GetOrSetAsync(CacheKeys.Forums(category.Id), async () =>
                {
                    var forums = await _dbContext.Forums
                        .Include(x => x.LastPost).ThenInclude(x => x.Member)
                        .Include(x => x.LastPost).ThenInclude(x => x.Topic)
                        .Where(x => x.CategoryId == category.Id && x.Status == StatusType.Published)
                        .OrderBy(x => x.SortOrder)
                        .ToListAsync();

                    return forums.Select(forum => new IndexPageModelToFilter.ForumModel
                    {
                        Id = forum.Id,
                        Name = forum.Name,
                        Description = forum.Description,
                        TotalTopics = forum.TopicsCount,
                        TotalReplies = forum.RepliesCount,
                        PermissionSetId = forum.PermissionSetId,
                        LastTopicId = forum.LastPost?.TopicId == null ? forum.LastPost?.Id : forum.LastPost?.Topic?.Id,
                        LastTopicTitle = forum.LastPost?.Title ?? forum.LastPost?.Topic?.Title,
                        LastPostTimeStamp = forum.LastPost?.TimeStamp,
                        LastPostMemberId = forum.LastPost?.Member?.Id,
                        LastPostMemberDisplayName = forum.LastPost?.Member?.DisplayName
                    }).ToList();
                });
            }

            return model;
        }

        public async Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, Guid forumId, QueryOptions options)
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
                }
            };

            var topicsQuery = _dbContext.Posts
                .Include(x => x.Member)
                .Include(x => x.LastReply).ThenInclude(x => x.Member)
                .Where(x =>
                    x.TopicId == null &&
                    x.ForumId == forum.Id &&
                    x.Status == StatusType.Published);

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                topicsQuery = topicsQuery
                    .Where(x => x.Title.Contains(options.Search) || x.Content.Contains(options.Search));
            }

            var topics = await topicsQuery
                .OrderByDescending(x => x.LastReply != null ? x.LastReply.TimeStamp : x.TimeStamp)
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
                    TimeStamp = topic.TimeStamp,
                    GravatarHash = _gravatarService.HashEmailForGravatar(topic.Member.Email),
                    MostRecentMemberId = topic.LastReply?.MemberId ?? topic.MemberId,
                    MostRecentMemberDisplayName = topic.LastReply?.Member?.DisplayName ?? topic.Member.DisplayName,
                    MostRecentTimeStamp = topic.LastReply?.TimeStamp ?? topic.TimeStamp
                })
                .ToList();

            var totalRecordsQuery = _dbContext.Posts
                .Where(x =>
                    x.TopicId == null &&
                    x.ForumId == forum.Id &&
                    x.Status == StatusType.Published);

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                totalRecordsQuery = totalRecordsQuery
                    .Where(x => x.Title.Contains(options.Search) || x.Content.Contains(options.Search));
            }

            var totalRecords = await totalRecordsQuery.CountAsync();

            result.Topics = new PaginatedData<ForumPageModel.TopicModel>(items, totalRecords, options.PageSize);

            return result;
        }

        public async Task<PostPageModel> BuildNewPostPageModelAsync(Guid siteId, Guid forumId)
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
                }
            };

            return result;
        }

        public async Task<PostPageModel> BuildEditPostPageModelAsync(Guid siteId, Guid forumId, Guid topicId)
        {
            var topic = await _dbContext.Posts
                .Include(x => x.Forum).ThenInclude(x => x.Category)
                .Include(x => x.Member)
                .FirstOrDefaultAsync(x =>
                    x.TopicId == null &&
                    x.Forum.Category.SiteId == siteId &&
                    x.Forum.Id == forumId &&
                    x.Id == topicId &&
                    x.Status == StatusType.Published);

            if (topic == null)
            {
                return null;
            }

            var result = new PostPageModel
            {
                Forum = new PostPageModel.ForumModel
                {
                    Id = topic.Forum.Id,
                    Name = topic.Forum.Name
                },
                Topic = new PostPageModel.TopicModel
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    Content = topic.Content,
                    MemberId = topic.Member.Id
                }
            };

            return result;
        }

        public async Task<TopicPageModel> BuildTopicPageModelAsync(Guid siteId, Guid forumId, Guid topicId, QueryOptions options)
        {
            var topic = await _dbContext.Posts
                .Include(x => x.Forum).ThenInclude(x => x.Category)
                .Include(x => x.Member)
                .FirstOrDefaultAsync(x =>
                    x.TopicId == null &&
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
                    TimeStamp = topic.TimeStamp,
                    UserId = topic.Member.UserId,
                    GravatarHash = _gravatarService.HashEmailForGravatar(topic.Member.Email)
                }
            };

            var replies = await _dbContext.Posts
                .Include(x => x.Member)
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
                    OriginalContent = reply.Content,
                    UserId = reply.Member.UserId,
                    MemberId = reply.Member.Id,
                    MemberDisplayName = reply.Member.DisplayName,
                    TimeStamp = reply.TimeStamp,
                    GravatarHash = _gravatarService.HashEmailForGravatar(reply.Member.Email)
            }).ToList();

            var totalRecords = await _dbContext.Posts
                .Where(x =>
                    x.TopicId == topic.Id &&
                    x.Status == StatusType.Published)
                .CountAsync();

            result.Replies = new PaginatedData<TopicPageModel.ReplyModel>(items, totalRecords, options.PageSize);

            return result;
        }

        public async Task<MemberPageModelToFilter> BuildMemberPageModelToFilterAsync(Guid siteId, Guid memberId)
        {
            var result = new MemberPageModelToFilter();

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

            result.MemberTopicModelsToFilter = await BuildMemberTopicModelsToFilterAsync(siteId, memberId, 0);

            result.TotalMemberTopics = member.TopicsCount;

            return result;
        }

        public async Task<MemberTopicModelsToFilter> BuildMemberTopicModelsToFilterAsync(Guid siteId, Guid memberId, int skip)
        {
            var result = new MemberTopicModelsToFilter();

            var topics = await _dbContext.Posts
                .Include(x => x.Forum)
                .Where(x =>
                    x.TopicId == null &&
                    x.Forum.Category.SiteId == siteId &&
                    (x.MemberId == memberId || x.LastReply.MemberId == memberId) &&
                    x.Status == StatusType.Published)
                .OrderByDescending(x => x.LastReply != null ? x.LastReply.TimeStamp : x.TimeStamp)
                .Skip(skip)
                .Take(5)
                .Select(t => new
                {
                    t.Id,
                    t.ForumId,
                    ForumName = t.Forum.Name,
                    t.Title,
                    t.TimeStamp,
                    LastReplyTimeStamp = t.LastReply.TimeStamp,
                    t.RepliesCount,
                    ForumPermissionSetId = t.Forum.PermissionSetId,
                    CategoryPermissionSetId = t.Forum.Category.PermissionSetId
                })
                .ToListAsync();

            foreach (var topic in topics)
            {
                result.Topics.Add(new MemberTopicModel
                {
                    Id = topic.Id,
                    ForumId = topic.ForumId,
                    ForumName = topic.ForumName,
                    Title = topic.Title,
                    TimeStamp = topic.LastReplyTimeStamp > DateTime.MinValue ? topic.LastReplyTimeStamp : topic.TimeStamp,
                    TotalReplies = topic.RepliesCount
                });

                result.TopicPermissions.Add(new TopicPermission
                {
                    TopicId = topic.Id,
                    PermissionSetId = topic.ForumPermissionSetId ?? topic.CategoryPermissionSetId
                });
            }

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
