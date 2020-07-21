using System;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Server.Services;
using Atlas.Shared.Models.Admin.ForumGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Atlas.Domain.ForumGroups.Commands;
using Atlas.Domain.ForumGroups;
using Atlas.Shared;

namespace Atlas.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    [Route("api/admin/forum-groups")]
    [ApiController]
    public class ForumGroupsController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IForumGroupService _forumGroupService;
        private readonly IForumGroupRules _forumGroupRules;
        private readonly IForumGroupModelBuilder _modelBuilder;

        public ForumGroupsController(IContextService contextService,
            IForumGroupService forumGroupService,
            IForumGroupRules forumGroupRules,
            IForumGroupModelBuilder modelBuilder)
        {
            _contextService = contextService;
            _forumGroupService = forumGroupService;
            _forumGroupRules = forumGroupRules;
            _modelBuilder = modelBuilder;
        }

        [HttpGet("index-model")]
        public async Task<IndexModel> Index()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildIndexModelAsync(site.Id);
        }

        [HttpGet("create")]
        public async Task<FormModel> Create()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildFormModelAsync(site.Id);
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormModel.ForumGroupModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new CreateForumGroup
            {
                Name = model.Name,
                PermissionSetId = model.PermissionSetId == Guid.Empty ? (Guid?)null : model.PermissionSetId,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _forumGroupService.CreateAsync(command);

            return Ok();
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<FormModel>> Edit(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();

            var result = await _modelBuilder.BuildFormModelAsync(site.Id, id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormModel.ForumGroupModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new UpdateForumGroup
            {
                Id = model.Id,
                Name = model.Name,
                PermissionSetId = model.PermissionSetId == Guid.Empty ? (Guid?)null : model.PermissionSetId,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _forumGroupService.UpdateAsync(command);

            return Ok();
        }

        [HttpPost("move-up")]
        public async Task<ActionResult> MoveUp([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new MoveForumGroup
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id,
                Direction = Direction.Up
            };

            await _forumGroupService.MoveAsync(command);

            return Ok();
        }

        [HttpPost("move-down")]
        public async Task<ActionResult> MoveDown([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new MoveForumGroup
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id,
                Direction = Direction.Down
            };

            await _forumGroupService.MoveAsync(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new DeleteForumGroup
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _forumGroupService.DeleteAsync(command);

            return Ok();
        }

        [HttpGet("is-name-unique/{name}")]
        public async Task<IActionResult> IsNameUnique(string name)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _forumGroupRules.IsNameUniqueAsync(site.Id, name);
            return Ok(isNameUnique);
        }

        [HttpGet("is-name-unique/{id}/{name}")]
        public async Task<IActionResult> IsNameUnique(Guid id, string name)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _forumGroupRules.IsNameUniqueAsync(site.Id, id, name);
            return Ok(isNameUnique);
        }
    }
}
