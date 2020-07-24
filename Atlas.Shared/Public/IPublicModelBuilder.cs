using System;
using System.Threading.Tasks;
using Atlas.Shared.Public.Models;

namespace Atlas.Shared.Public
{
    public interface IPublicModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
        Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, Guid forumId);
        Task<TopicPageModel> BuildNewTopicPageModelAsync(Guid siteId, Guid forumId);
    }
}
