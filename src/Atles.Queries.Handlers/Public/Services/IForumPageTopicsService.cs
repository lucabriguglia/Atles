using Atles.Models;
using Atles.Models.Public;
using Atles.Queries.Public;

namespace Atles.Queries.Handlers.Public.Services;

public interface IForumPageTopicsService
{
    Task<PaginatedData<ForumPageModel.TopicModel>> GetForumPageTopics(GetForumPageTopics query);
}