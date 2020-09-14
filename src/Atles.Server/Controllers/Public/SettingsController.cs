using System.Threading.Tasks;
using Atlas.Models.Public.Users;
using Atlas.Server.Services;
using Atles.Domain.Users;
using Atles.Domain.Users.Commands;
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
        private readonly IUserService _userService;
        private readonly IUserRules _userRules;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(IContextService contextService, 
            IUserModelBuilder modelBuilder,
            IUserService userService, 
            IUserRules userRules, 
            ILogger<SettingsController> logger)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _userService = userService;
            _userRules = userRules;
            _logger = logger;
        }

        [HttpGet("edit")]
        public async Task<ActionResult<SettingsPageModel>> Edit()
        {
            var user = await _contextService.CurrentUserAsync();

            var model = await _modelBuilder.BuildSettingsPageModelAsync(user.Id);

            return model;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(SettingsPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            if (model.User.Id != user.Id || user.IsSuspended)
            {
                _logger.LogWarning("Unauthorized access to update settings.", new
                {
                    SiteId = site.Id,
                    UserId = model.User?.Id,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            var command = new UpdateUser
            {
                Id = user.Id,
                DisplayName = model.User.DisplayName,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _userService.UpdateAsync(command);

            return Ok();
        }

        [HttpGet("is-display-name-unique/{name}")]
        public async Task<IActionResult> IsDisplayNameUnique(string name)
        {
            var isNameUnique = await _userRules.IsDisplayNameUniqueAsync(name);
            return Ok(isNameUnique);
        }
    }
}
