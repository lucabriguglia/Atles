using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Domain.Members;
using Atlas.Domain.Members.Commands;
using Atlas.Domain.PermissionSets;
using Atlas.Models;
using Atlas.Models.Public;
using Atlas.Models.Public.Members;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers
{
    [Route("api/public/member")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IPublicModelBuilder _modelBuilder;
        private readonly IPermissionModelBuilder _permissionModelBuilder;
        private readonly ISecurityService _securityService;
        private readonly IMemberService _memberService;

        public MemberController(IContextService contextService, 
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

            var modelToFilter = await _modelBuilder.BuildMemberPageModelToFilterAsync(site.Id, memberId);

            if (modelToFilter == null)
            {
                return NotFound();
            }

            var result = new MemberPageModel
            {
                Member = new MemberModel
                {
                    Id = modelToFilter.Member.Id,
                    DisplayName = modelToFilter.Member.DisplayName,
                    TotalTopics = modelToFilter.Member.TotalTopics,
                    TotalReplies = modelToFilter.Member.TotalReplies,
                    GravatarHash = modelToFilter.Member.GravatarHash
                },
                LastTopics = await GetFilteredMemberTopicModels(site.Id, modelToFilter.MemberTopicModelsToFilter)
            };

            const int maxNumberOfTopicsToReturn = 5;
            var repeat = 0;

            while (result.LastTopics.Count < maxNumberOfTopicsToReturn && 
                   result.LastTopics.Count < modelToFilter.TotalMemberTopics &&
                   modelToFilter.TotalMemberTopics >= (repeat + 1) * maxNumberOfTopicsToReturn &&
                   repeat < 3)
            {
                repeat++;
                var furtherMemberTopicModelsToFilter = await _modelBuilder.BuildMemberTopicModelsToFilterAsync(site.Id, memberId, repeat * maxNumberOfTopicsToReturn);
                var furtherTopics = await GetFilteredMemberTopicModels(site.Id, furtherMemberTopicModelsToFilter);
                foreach (var furtherTopic in furtherTopics)
                {
                    result.LastTopics.Add(furtherTopic);
                    if (result.LastTopics.Count == maxNumberOfTopicsToReturn)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        private async Task<IList<MemberTopicModel>> GetFilteredMemberTopicModels(Guid siteId, MemberTopicModelsToFilter memberTopicModelsToFilter)
        {
            var result = new List<MemberTopicModel>();

            foreach (var topic in memberTopicModelsToFilter.Topics)
            {
                var topicPermission = memberTopicModelsToFilter.TopicPermissions.FirstOrDefault(x => x.TopicId == topic.Id);
                var permissions = await _permissionModelBuilder.BuildPermissionModels(siteId, topicPermission.PermissionSetId);
                var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);
                if (!canViewTopics) continue;
                var canRead = _securityService.HasPermission(PermissionType.Read, permissions);
                result.Add(new MemberTopicModel
                {
                    Id = topic.Id,
                    ForumId = topic.ForumId,
                    Title = topic.Title,
                    TimeStamp = topic.TimeStamp,
                    TotalReplies = topic.TotalReplies,
                    CanRead = canRead
                });
            }

            return result;
        }

        [Authorize]
        [HttpPost("update-display-name")]
        public async Task<ActionResult> Update(string displayName)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new UpdateMember
            {
                Id = member.Id,
                DisplayName = displayName,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _memberService.UpdateAsync(command);

            return Ok();
        }
    }
}
