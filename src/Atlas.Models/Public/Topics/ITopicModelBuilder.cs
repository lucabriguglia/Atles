using System;
using System.Threading.Tasks;

namespace Atlas.Models.Public.Topics
{
    public interface ITopicModelBuilder
    {
        Task<TopicPageModel> BuildTopicPageModelAsync(Guid siteId, Guid forumId, Guid topicId, QueryOptions options);
        Task<PaginatedData<TopicPageModel.ReplyModel>> BuildTopicPageModelRepliesAsync(Guid topicId, QueryOptions options);
    }
}