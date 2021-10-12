using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Domain.PermissionSets;
using Atles.Models.Public.Users;
using Atles.Reporting.Public.Queries;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenCqrs;

namespace Atles.Server.Controllers.Public
{
    [Route("api/public/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly ISecurityService _securityService;
        private readonly ILogger<UsersController> _logger;
        private readonly ISender _sender;

        public UsersController(IContextService contextService,
            ISecurityService securityService, 
            ILogger<UsersController> logger,
            ISender sender)
        {
            _contextService = contextService;
            _securityService = securityService;
            _logger = logger;
            _sender = sender;
        }

        [HttpGet]
        [Route("")]
        [Route("{id}")]
        public async Task<ActionResult<UserPageModel>> Index(Guid? id = null)
        {
            var site = await _contextService.CurrentSiteAsync();

            var userId = Guid.Empty;

            if (id == null)
            {
                var user = await _contextService.CurrentUserAsync();

                if (user != null)
                {
                    userId = user.Id;
                }
            }
            else
            {
                userId = id.Value;
            }

            var currentForums = await _contextService.CurrentForumsAsync();

            var accessibleForumIds = new List<Guid>();

            foreach (var forum in currentForums)
            {
                var permissions = await _sender.Send(new GetPermissions { SiteId = site.Id, PermissionSetId = forum.PermissionSetId });
                var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
                var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);
                var canViewRead = _securityService.HasPermission(PermissionType.Read, permissions);
                if (canViewForum && canViewTopics && canViewRead)
                {
                    accessibleForumIds.Add(forum.Id);
                }
            }

            var model = await _sender.Send(new GetUserPage { SiteId = site.Id, UserId = userId, AccessibleForumIds = accessibleForumIds });

            if (model == null)
            {
                _logger.LogWarning("User not found.", new
                {
                    SiteId = site.Id,
                    MemebrId = userId,
                    User = User.Identity.Name
                });

                return NotFound();
            }

            return model;
        }
    }
}
