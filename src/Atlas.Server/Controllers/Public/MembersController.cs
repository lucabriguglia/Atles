using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Domain.PermissionSets;
using Atlas.Models;
using Atlas.Models.Public;
using Atlas.Models.Public.Members;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers.Public
{
    [Route("api/public/members")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IMemberModelBuilder _modelBuilder;
        private readonly IPermissionModelBuilder _permissionModelBuilder;
        private readonly ISecurityService _securityService;

        public MembersController(IContextService contextService,
            IMemberModelBuilder modelBuilder, 
            IPermissionModelBuilder permissionModelBuilder, 
            ISecurityService securityService)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _permissionModelBuilder = permissionModelBuilder;
            _securityService = securityService;
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

            var result = await _modelBuilder.BuildMemberPageModelAsync(memberId, accessibleForumIds);

            return result;
        }
    }
}
