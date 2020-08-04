using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Models.Public.Members;

namespace Atlas.Models.Public
{
    public interface IPublicModelBuilder
    {
        Task<IndexPageModelToFilter> BuildIndexPageModelToFilterAsync(Guid siteId);
        Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, Guid forumId, QueryOptions options);
        Task<PostPageModel> BuildNewPostPageModelAsync(Guid siteId, Guid forumId);
        Task<PostPageModel> BuildEditPostPageModelAsync(Guid siteId, Guid forumId, Guid topicId);
        Task<TopicPageModel> BuildTopicPageModelAsync(Guid siteId, Guid forumId, Guid topicId, QueryOptions options);
        Task<MemberPageModelToFilter> BuildMemberPageModelToFilterAsync(Guid siteId, Guid memberId);
        Task<MemberTopicModelsToFilter> BuildMemberTopicModelsToFilterAsync(Guid siteId, Guid memberId, int skip);
    }
}
