using System;
using System.Threading.Tasks;
using Atlas.Domain.Members;
using Atlas.Domain.Members.Commands;
using Atlas.Models.Admin.Members;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    [Route("api/admin/categories")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IMemberService _memberService;
        private readonly IMemberRules _memberRules;
        private readonly IMemberModelBuilder _modelBuilder;

        public MembersController(IContextService contextService,
            IMemberService memberService,
            IMemberRules memberRules,
            IMemberModelBuilder modelBuilder)
        {
            _contextService = contextService;
            _memberService = memberService;
            _memberRules = memberRules;
            _modelBuilder = modelBuilder;
        }

        [HttpGet("list")]
        public async Task<IndexPageModel> List()
        {
            // TODO: Add pagination and search

            return await _modelBuilder.BuildIndexPageModelAsync();
        }

        [HttpGet("create")]
        public async Task<FormComponentModel> Create()
        {
            return await _modelBuilder.BuildFormModelAsync();
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormComponentModel.MemberModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            // TODO: Register user using user manager

            var command = new CreateMember
            {
                UserId = Guid.NewGuid().ToString(),
                DisplayName = model.DisplayName,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _memberService.CreateAsync(command);

            return Ok();
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<FormComponentModel>> Edit(Guid id)
        {
            var result = await _modelBuilder.BuildFormModelAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormComponentModel.MemberModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new UpdateMember
            {
                Id = model.Id,
                DisplayName = model.DisplayName,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _memberService.UpdateAsync(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new DeleteMember
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _memberService.DeleteAsync(command);

            return Ok();
        }

        [HttpGet("is-display-name-unique/{name}")]
        public async Task<IActionResult> IsDisplayNameUnique(string name)
        {
            var isNameUnique = await _memberRules.IsDisplayNameUniqueAsync(name);
            return Ok(isNameUnique);
        }

        [HttpGet("is-display-name-unique/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(string name, Guid id)
        {
            var isNameUnique = await _memberRules.IsDisplayNameUniqueAsync(name, id);
            return Ok(isNameUnique);
        }
    }
}
