using Atles.Core;
using Atles.Domain;
using Atles.Models;
using Atles.Models.Public;
using Atles.Queries.Public;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Public;

[Route("api/public/forums")]
public class ForumsController : SiteControllerBase
{
    private readonly ISecurityService _securityService;
    private readonly ILogger<ForumsController> _logger;
    private readonly IDispatcher _dispatcher;

    public ForumsController(ISecurityService securityService,
        ILogger<ForumsController> logger,
        IDispatcher dispatcher) : base(dispatcher)
    {
        _securityService = securityService;
        _logger = logger;
        _dispatcher = dispatcher;
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ForumPageModel>> Forum(string slug, [FromQuery] int? page = 1, [FromQuery] string search = null)
    {
        var getForumPageResult = await _dispatcher.Get(new GetForumPage { SiteId = CurrentSite.Id, Slug = slug, Options = new QueryOptions(page, search) });
        var model = getForumPageResult.AsT0; // TODO: Refactoring

        if (model == null)
        {
            _logger.LogWarning("Forum not found.");
            return NotFound();
        }

        var getPermissionsResult = await _dispatcher.Get(new GetPermissions { SiteId = CurrentSite.Id, ForumId = model.Forum.Id });
        var permissions = getPermissionsResult.AsT0; // TODO: Refactoring

        var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
        var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);

        if (!canViewForum || !canViewTopics)
        {
            _logger.LogWarning("Unauthorized access to forum.");
            return Unauthorized();
        }

        model.CanRead = _securityService.HasPermission(PermissionType.Read, permissions);
        model.CanStart = _securityService.HasPermission(PermissionType.Start, permissions) && !CurrentUser.IsSuspended;

        return model;
    }

    [HttpGet("{id}/topics")]
    public async Task<ActionResult<PaginatedData<ForumPageModel.TopicModel>>> Topics(Guid id, [FromQuery] int? page = 1, [FromQuery] string search = null)
    {
        var getPermissionsResult = await _dispatcher.Get(new GetPermissions { SiteId = CurrentSite.Id, ForumId = id });
        var permissions = getPermissionsResult.AsT0; // TODO: Refactoring

        var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
        var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);

        if (!canViewForum || !canViewTopics)
        {
            _logger.LogWarning("Unauthorized access to forum topics.");
            return Unauthorized();
        }

        var result = await _dispatcher.Get(new GetForumPageTopics { SiteId = CurrentSite.Id, ForumId = id, Options = new QueryOptions(page, search) });

        return result.AsT0; // TODO: Refactoring
    }
}
