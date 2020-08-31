using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Domain.PermissionSets;
using Atlas.Models.Public;
using Atlas.Models.Public.Users;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atlas.Server.Controllers.Public
{
    [Route("api/public/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IUserModelBuilder _modelBuilder;
        private readonly IPermissionModelBuilder _permissionModelBuilder;
        private readonly ISecurityService _securityService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IContextService contextService,
            IUserModelBuilder modelBuilder, 
            IPermissionModelBuilder permissionModelBuilder, 
            ISecurityService securityService, 
            ILogger<UsersController> logger)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _permissionModelBuilder = permissionModelBuilder;
            _securityService = securityService;
            _logger = logger;
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
                var permissions = await _permissionModelBuilder.BuildPermissionModels(site.Id, forum.PermissionSetId);
                var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
                var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);
                var canViewRead = _securityService.HasPermission(PermissionType.Read, permissions);
                if (canViewForum && canViewTopics && canViewRead)
                {
                    accessibleForumIds.Add(forum.Id);
                }
            }

            var model = await _modelBuilder.BuildUserPageModelAsync(userId, accessibleForumIds);

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
