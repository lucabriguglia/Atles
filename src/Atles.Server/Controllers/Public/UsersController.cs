using Atles.Core;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Public;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Public;

[Route("api/public/users")]
public class UsersController : SiteControllerBase
{
    private readonly ISecurityService _securityService;
    private readonly ILogger<UsersController> _logger;
    private readonly IDispatcher _dispatcher;

    public UsersController(ISecurityService securityService,
        ILogger<UsersController> logger,
        IDispatcher dispatcher) : base(dispatcher)
    {
        _securityService = securityService;
        _logger = logger;
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [Route("")]
    [Route("{id}")]
    public async Task<ActionResult<UserPageModel>> Index(Guid? id = null)
    {
        var userId = Guid.Empty;

        if (id == null)
        {
            if (CurrentUser != null)
            {
                userId = CurrentUser.Id;
            }
        }
        else
        {
            userId = id.Value;
        }

        var accessibleForumIds = new List<Guid>();

        foreach (var forum in CurrentForums)
        {
            var permissions = await _dispatcher.Get(new GetPermissions {SiteId = CurrentSite.Id, PermissionSetId = forum.PermissionSetId});
            var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions.AsT0);
            var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions.AsT0);
            var canViewRead = _securityService.HasPermission(PermissionType.Read, permissions.AsT0);
            if (canViewForum && canViewTopics && canViewRead)
            {
                accessibleForumIds.Add(forum.Id);
            }
        }

        var model = await _dispatcher.Get(new GetUserPage
        {
            SiteId = CurrentSite.Id, 
            UserId = userId, 
            AccessibleForumIds = accessibleForumIds
        });

        if (model?.AsT0 != null)
        {
            return model.AsT0;
        }

        _logger.LogWarning("User not found.");

        return NotFound();
    }
}
