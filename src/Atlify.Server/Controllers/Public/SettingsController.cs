using System.Threading.Tasks;
using Atlify.Models.Public.Members;
using Atlify.Server.Services;
using Atlify.Domain.Members;
using Atlify.Domain.Members.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atlify.Server.Controllers.Public
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
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(IContextService contextService, 
            IMemberModelBuilder modelBuilder,
            IMemberService memberService, 
            IMemberRules memberRules, 
            ILogger<SettingsController> logger)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _memberService = memberService;
            _memberRules = memberRules;
            _logger = logger;
        }

        [HttpGet("edit")]
        public async Task<ActionResult<SettingsPageModel>> Edit()
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

            if (model.Member.Id != member.Id || member.IsSuspended)
            {
                _logger.LogWarning("Unauthorized access to update settings.", new
                {
                    SiteId = site.Id,
                    MemberId = model.Member?.Id,
                    User = User.Identity.Name
                });

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
