using System;
using System.Threading.Tasks;

namespace Atlas.Models.Public
{
    public interface IPublicModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
        Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, Guid forumId, PaginationOptions options);
        Task<PostPageModel> BuildNewPostPageModelAsync(Guid siteId, Guid forumId);
        Task<PostPageModel> BuildEditPostPageModelAsync(Guid siteId, Guid forumId, Guid topicId);
        Task<TopicPageModel> BuildTopicPageModelAsync(Guid siteId, Guid forumId, Guid topicId, PaginationOptions options);
    }
}
