using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Domain.Members;
using Atlas.Domain.PermissionSets;
using Atlas.Models;
using Atlas.Models.Public;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers.Public
{
    [Route("api/public/members")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IPublicModelBuilder _modelBuilder;
        private readonly IPermissionModelBuilder _permissionModelBuilder;
        private readonly ISecurityService _securityService;
        private readonly IMemberService _memberService;

        public MembersController(IContextService contextService, 
            IPublicModelBuilder modelBuilder, 
            IPermissionModelBuilder permissionModelBuilder, 
            ISecurityService securityService, 
            IMemberService memberService)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _permissionModelBuilder = permissionModelBuilder;
            _securityService = securityService;
            _memberService = memberService;
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

            var indexModelToFilter = await _modelBuilder.BuildIndexPageModelToFilterAsync(site.Id);

            var accessibleForumIds = new List<Guid>();

            foreach (var category in indexModelToFilter.Categories)
            {
                foreach (var forum in category.Forums)
                {
                    var permissionSetId = forum.PermissionSetId ?? category.PermissionSetId;
                    var permissions = await _permissionModelBuilder.BuildPermissionModels(site.Id, permissionSetId);
                    var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
                    var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);
                    var canViewRead = _securityService.HasPermission(PermissionType.Read, permissions);
                    if (canViewForum && canViewTopics && canViewRead)
                    {
                        accessibleForumIds.Add(forum.Id);
                    }
                }
            }

            var result = await _modelBuilder.BuildMemberPageModelAsync(memberId, accessibleForumIds);

            return result;
        }
    }
}
