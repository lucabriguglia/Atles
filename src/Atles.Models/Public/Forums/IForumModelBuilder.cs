using System;
using System.Threading.Tasks;

namespace Atlas.Models.Public.Forums
{
    public interface IForumModelBuilder
    {
        Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, string forumSlug, QueryOptions options);
        Task<PaginatedData<ForumPageModel.TopicModel>> BuildForumPageModelTopicsAsync(Guid forumId, QueryOptions options);
    }
}