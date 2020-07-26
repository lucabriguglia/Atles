using System;
using System.Threading.Tasks;

namespace Atlas.Models.Public
{
    public interface IPublicModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
        Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, Guid forumId, PaginationOptions options);
        Task<PostPageModel> BuildPostPageModelAsync(Guid siteId, Guid forumId);
        Task<TopicPageModel> BuildTopicPageModelAsync(Guid siteId, Guid forumId, Guid topicId);
    }
}
