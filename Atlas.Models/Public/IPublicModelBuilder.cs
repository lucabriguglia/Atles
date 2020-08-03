using System;
using System.Threading.Tasks;

namespace Atlas.Models.Public
{
    public interface IPublicModelBuilder
    {
        Task<IndexPageModelToFilter> BuildIndexPageModelToFilterAsync(Guid siteId);
        Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, Guid forumId, QueryOptions options);
        Task<PostPageModel> BuildNewPostPageModelAsync(Guid siteId, Guid forumId);
        Task<PostPageModel> BuildEditPostPageModelAsync(Guid siteId, Guid forumId, Guid topicId);
        Task<TopicPageModel> BuildTopicPageModelAsync(Guid siteId, Guid forumId, Guid topicId, QueryOptions options);
        Task<MemberPageModel> BuildMemberPageModelAsync(Guid siteId, Guid memberId);
    }
}
