using System;
using System.Threading.Tasks;
using Atles.Commands.Posts;
using Atles.Core;
using Atles.Domain;
using Atles.Reporting.Models.Public;
using Atles.Reporting.Models.Public.Queries;
using Atles.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atles.Server.Controllers.Public
{
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
        public async Task<ActionResult<UserTopicReactionsModel>> TopicReactions(Guid topicId)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var query = new GetUserTopicReactions
            {
                SiteId = site.Id,
                UserId = user.Id,
                TopicId = topicId
            };

            var model = await _dispatcher.Get(query);

            if (model != null)
            {
                return model;
            }

            _logger.LogWarning("User not found.");

            return NotFound();
        }

        [HttpPost("add-reaction/{forumId}/{postId}")]
        public async Task<ActionResult> AddReaction(Guid forumId, Guid postId, [FromBody] PostReactionType postReactionType)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var permissions = await _dispatcher.Get(new GetPermissions { SiteId = site.Id, ForumId = forumId });
            var canReact = _securityService.HasPermission(PermissionType.Reactions, permissions) && !user.IsSuspended;
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !user.IsSuspended;

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
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpPost("remove-reaction/{forumId}/{postId}")]
        public async Task<ActionResult> RemoveReaction(Guid forumId, Guid postId)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var permissions = await _dispatcher.Get(new GetPermissions { SiteId = site.Id, ForumId = forumId });
            var canReact = _securityService.HasPermission(PermissionType.Reactions, permissions) && !user.IsSuspended;
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !user.IsSuspended;

            if (!canReact && !canModerate)
            {
                _logger.LogWarning("Unauthorized access to remove reaction.");
                return Unauthorized();
            }

            var command = new RemovePostReaction
            {
                PostId = postId,
                ForumId = forumId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }
    }
}