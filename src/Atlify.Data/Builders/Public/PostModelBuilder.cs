using System;
using System.Threading.Tasks;
using Atlify.Models.Public.Posts;
using Atlify.Data.Caching;
using Atlify.Domain;
using Microsoft.EntityFrameworkCore;

namespace Atlify.Data.Builders.Public
{
    public class PostModelBuilder : IPostModelBuilder
    {
        private readonly AtlifyDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IGravatarService _gravatarService;

        public PostModelBuilder(AtlifyDbContext dbContext,
            ICacheManager cacheManager,
            IGravatarService gravatarService)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _gravatarService = gravatarService;
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
                    Name = forum.Name,
                    Slug = forum.Slug
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
                    Name = topic.Forum.Name,
                    Slug = topic.Forum.Slug
                },
                Topic = new PostPageModel.TopicModel
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    Slug = topic.Slug,
                    Content = topic.Content,
                    MemberId = topic.Member.Id,
                    Locked = topic.Locked
                }
            };

            return result;
        }
    }
}