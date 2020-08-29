using System.Threading.Tasks;
using Atlas.Domain.Users;
using Atlas.Domain.Users.Commands;
using Atlas.Models.Public.Users;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atlas.Server.Controllers.Public
{
    [Authorize]
    [Route("api/public/settings")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IUserModelBuilder _modelBuilder;
        private readonly IUserService _memberService;
        private readonly IUserRules _memberRules;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(IContextService contextService, 
            IUserModelBuilder modelBuilder,
            IUserService memberService, 
            IUserRules memberRules, 
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
            var member = await _contextService.CurrentUserAsync();

            var model = await _modelBuilder.BuildSettingsPageModelAsync(member.Id);

            return model;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(SettingsPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentUserAsync();

            if (model.User.Id != member.Id || member.IsSuspended)
            {
                _logger.LogWarning("Unauthorized access to update settings.", new
                {
                    SiteId = site.Id,
                    MemberId = model.User?.Id,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            var command = new UpdateUser
            {
                Id = member.Id,
                DisplayName = model.User.DisplayName,
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
