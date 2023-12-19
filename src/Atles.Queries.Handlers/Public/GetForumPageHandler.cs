using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Core.Results.Types;
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
    private readonly ISecurityService _securityService;
    private readonly IPermissionsService _permissionsService;

    public GetForumPageHandler(
        AtlesDbContext dbContext,
        IForumPageTopicsService forumPageTopicsService,
        ISecurityService securityService, 
        IPermissionsService permissionsService)
    {
        _dbContext = dbContext;
        _forumPageTopicsService = forumPageTopicsService;
        _securityService = securityService;
        _permissionsService = permissionsService;
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
            return new Failure(FailureType.NotFound, "Forum", $"Forum with slug {query.Slug} not found.");
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

        var permissions = await _permissionsService.GetPermissions(query.SiteId, forum.Id, null);

        var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
        var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);

        if (!canViewForum || !canViewTopics)
        {
            return new Failure(FailureType.Unauthorized, "Forum", $"User can't access forum with slug {query.Slug}.");
        }

        result.CanRead = _securityService.HasPermission(PermissionType.Read, permissions);
        result.CanStart = _securityService.HasPermission(PermissionType.Start, permissions) /*TODO: && !CurrentUser.IsSuspended*/;

        return result;
    }
}
