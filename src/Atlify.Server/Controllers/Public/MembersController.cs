using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlify.Models.Public;
using Atlify.Models.Public.Members;
using Atlify.Server.Services;
using Atlify.Domain.PermissionSets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atlify.Server.Controllers.Public
{
    [Route("api/public/members")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IMemberModelBuilder _modelBuilder;
        private readonly IPermissionModelBuilder _permissionModelBuilder;
        private readonly ISecurityService _securityService;
        private readonly ILogger<MembersController> _logger;

        public MembersController(IContextService contextService,
            IMemberModelBuilder modelBuilder, 
            IPermissionModelBuilder permissionModelBuilder, 
            ISecurityService securityService, 
            ILogger<MembersController> logger)
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
        public async Task<ActionResult<MemberPageModel>> Index(Guid? id = null)
        {
            var site = await _contextService.CurrentSiteAsync();

            var memberId = Guid.Empty;

            if (id == null)
            {
                var member = await _contextService.CurrentMemberAsync();

                if (member != null)
                {
                    memberId = member.Id;
                }
            }
            else
            {
                memberId = id.Value;
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

            var model = await _modelBuilder.BuildMemberPageModelAsync(memberId, accessibleForumIds);

            if (model == null)
            {
                _logger.LogWarning("Member not found.", new
                {
                    SiteId = site.Id,
                    MemebrId = memberId,
                    User = User.Identity.Name
                });

                return NotFound();
            }

            return model;
        }
    }
}
