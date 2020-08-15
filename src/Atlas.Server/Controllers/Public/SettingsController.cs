using System.Threading.Tasks;
using Atlas.Domain.Members;
using Atlas.Domain.Members.Commands;
using Atlas.Models.Public.Members;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers.Public
{
    [Authorize]
    [Route("api/public/settings")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IMemberModelBuilder _modelBuilder;
        private readonly IMemberService _memberService;
        private readonly IMemberRules _memberRules;

        public SettingsController(IContextService contextService, 
            IMemberModelBuilder modelBuilder,
            IMemberService memberService, 
            IMemberRules memberRules)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _memberService = memberService;
            _memberRules = memberRules;
        }

        [HttpGet("edit")]
        public async Task<SettingsPageModel> Edit()
        {
            var member = await _contextService.CurrentMemberAsync();

            var model = await _modelBuilder.BuildSettingsPageModelAsync(member.Id);

            return model;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(SettingsPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            if (model.Member.Id != member.Id)
            {
                return Unauthorized();
            }

            var command = new UpdateMember
            {
                Id = member.Id,
                DisplayName = model.Member.DisplayName,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _memberService.UpdateAsync(command);

            return Ok();
        }

        [HttpGet("is-display-name-unique/{name}")]
        public async Task<IActionResult> IsDisplayNameUnique(string name)
        {
            var isNameUnique = await _memberRules.IsDisplayNameUniqueAsync(name);
            return Ok(isNameUnique);
        }
    }
}
