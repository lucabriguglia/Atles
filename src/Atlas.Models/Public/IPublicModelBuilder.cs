using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Models.Public
{
    public interface IPublicModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
        Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, Guid forumId, QueryOptions options);
        Task<PaginatedData<ForumPageModel.TopicModel>> BuildForumPageModelTopicsAsync(Guid forumId, QueryOptions options);
        Task<PostPageModel> BuildNewPostPageModelAsync(Guid siteId, Guid forumId);
        Task<PostPageModel> BuildEditPostPageModelAsync(Guid siteId, Guid forumId, Guid topicId);
        Task<TopicPageModel> BuildTopicPageModelAsync(Guid siteId, Guid forumId, Guid topicId, QueryOptions options);
        Task<PaginatedData<TopicPageModel.ReplyModel>> BuildTopicPageModelRepliesAsync(Guid topicId, QueryOptions options);
        Task<MemberPageModel> BuildMemberPageModelAsync(Guid memberId, IList<Guid> forumIds);
        Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid memberId);
        Task<SearchPageModel> BuildSearchPageModelAsync(Guid siteId, IList<Guid> forumIds, QueryOptions options);
    }
}
