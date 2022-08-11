using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Models;
using Atles.Models.Public;
using Atles.Queries.Handlers.Public.Services;
using Atles.Queries.Public;

namespace Atles.Queries.Handlers.Public;

public class GetForumPageTopicsHandler : IQueryHandler<GetForumPageTopics, PaginatedData<ForumPageModel.TopicModel>>
{
    private readonly IForumPageTopicsService _forumPageTopicsService;

    public GetForumPageTopicsHandler(IForumPageTopicsService forumPageTopicsService)
    {
        _forumPageTopicsService = forumPageTopicsService;
    }

    public async Task<QueryResult<PaginatedData<ForumPageModel.TopicModel>>> Handle(GetForumPageTopics query)
    {
        return await _forumPageTopicsService.GetForumPageTopics(query);
    }
}