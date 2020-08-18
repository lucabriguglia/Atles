using System;
using System.Threading.Tasks;

namespace Atlify.Models.Public.Topics
{
    public interface ITopicModelBuilder
    {
        Task<TopicPageModel> BuildTopicPageModelAsync(Guid siteId, string forumSlug, string topicSlug, QueryOptions options);
        Task<PaginatedData<TopicPageModel.ReplyModel>> BuildTopicPageModelRepliesAsync(Guid topicId, QueryOptions options);
    }
}