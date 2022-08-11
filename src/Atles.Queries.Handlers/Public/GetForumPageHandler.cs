using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Handlers.Public.Services;
using Atles.Queries.Public;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public;

public class GetForumPageHandler : IQueryHandler<GetForumPage, ForumPageModel>
{
    private readonly AtlesDbContext _dbContext;
    private readonly IForumPageTopicsService _forumPageTopicsService;

    public GetForumPageHandler(AtlesDbContext dbContext, IForumPageTopicsService forumPageTopicsService)
    {
        _dbContext = dbContext;
        _forumPageTopicsService = forumPageTopicsService;
    }

    public async Task<QueryResult<ForumPageModel>> Handle(GetForumPage query)
    {
        var forum = await _dbContext.Forums
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x =>
                x.Slug == query.Slug &&
                x.Category.SiteId == query.SiteId &&
                x.Status == ForumStatusType.Published);

        if (forum == null)
        {
            return null;
        }

        var topics = await _forumPageTopicsService.GetForumPageTopics(new GetForumPageTopics
        {
            ForumId = forum.Id,
            Options = query.Options
        });

        var result = new ForumPageModel
        {
            Forum = new ForumPageModel.ForumModel
            {
                Id = forum.Id,
                Name = forum.Name,
                Description = forum.Description,
                Slug = forum.Slug
            },
            Topics = topics
        };

        return result;
    }
}
