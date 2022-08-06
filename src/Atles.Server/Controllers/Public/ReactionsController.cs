using Atles.Commands.Posts;
using Atles.Core;
using Atles.Domain;
using Atles.Queries.Public;
using Atles.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Public;

[Authorize]
[Route("api/public/reactions")]
[ApiController]
public class ReactionsController : SiteControllerBase
{
    private readonly IDispatcher _dispatcher;
    private readonly ISecurityService _securityService;
    private readonly ILogger<ReactionsController> _logger;

    public ReactionsController(IDispatcher dispatcher, ISecurityService securityService, ILogger<ReactionsController> logger) : base(dispatcher)
    {
        _dispatcher = dispatcher;
        _securityService = securityService;
        _logger = logger;
    }

    [HttpGet("topic-reactions/{topicId}")]
    public async Task<ActionResult> TopicReactions(Guid topicId)
    {
        return await ProcessGet(new GetUserTopicReactions
        {
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id,
            TopicId = topicId
        });
    }

    [HttpPost("add-reaction/{forumId}/{postId}")]
    public async Task<ActionResult> AddReaction(Guid forumId, Guid postId, [FromBody] PostReactionType postReactionType)
    {
        var permissions = await _dispatcher.Get(new GetPermissions { SiteId = CurrentSite.Id, ForumId = forumId });
        var canReact = _securityService.HasPermission(PermissionType.Reactions, permissions.AsT0) && !CurrentUser.IsSuspended; // TODO: Refactoring
        var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions.AsT0) && !CurrentUser.IsSuspended; // TODO: Refactoring

        if (!canReact && !canModerate)
        {
            _logger.LogWarning("Unauthorized access to add reaction.");
            return Unauthorized();
        }

        var command = new AddPostReaction
        {
            PostId = postId,
            ForumId = forumId,
            Type = postReactionType,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpPost("remove-reaction/{forumId}/{postId}")]
    public async Task<ActionResult> RemoveReaction(Guid forumId, Guid postId)
    {
        // TODO: Refactoring

        var permissions = await _dispatcher.Get(new GetPermissions { SiteId = CurrentSite.Id, ForumId = forumId });
        var canReact = _securityService.HasPermission(PermissionType.Reactions, permissions.AsT0) && !CurrentUser.IsSuspended;
        var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions.AsT0) && !CurrentUser.IsSuspended;

        if (!canReact && !canModerate)
        {
            _logger.LogWarning("Unauthorized access to remove reaction.");
            return Unauthorized();
        }

        var command = new RemovePostReaction
        {
            PostId = postId,
            ForumId = forumId,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }
}
