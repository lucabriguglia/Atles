using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Domain.Models.PermissionSets;
using Atles.Infrastructure;
using Atles.Reporting.Models.Public;
using Atles.Reporting.Models.Public.Queries;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atles.Server.Controllers.Public
{
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
            var site = await CurrentSite();

            var userId = Guid.Empty;

            if (id == null)
            {
                var user = await CurrentUser();

                if (user != null)
                {
                    userId = user.Id;
                }
            }
            else
            {
                userId = id.Value;
            }

            var currentForums = await CurrentForums();

            var accessibleForumIds = new List<Guid>();

            foreach (var forum in currentForums)
            {
                var permissions = await _dispatcher.Get(new GetPermissions { SiteId = site.Id, PermissionSetId = forum.PermissionSetId });
                var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
                var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);
                var canViewRead = _securityService.HasPermission(PermissionType.Read, permissions);
                if (canViewForum && canViewTopics && canViewRead)
                {
                    accessibleForumIds.Add(forum.Id);
                }
            }

            var model = await _dispatcher.Get(new GetUserPage { SiteId = site.Id, UserId = userId, AccessibleForumIds = accessibleForumIds });

            if (model == null)
            {
                _logger.LogWarning("User not found.");
                return NotFound();
            }

            return model;
        }
    }
}
