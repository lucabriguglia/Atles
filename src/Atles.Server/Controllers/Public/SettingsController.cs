using System.Threading.Tasks;
using Atles.Domain.Categories.Rules;
using Atles.Domain.Users.Commands;
using Atles.Models.Public.Users;
using Atles.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenCqrs;

namespace Atles.Server.Controllers.Public
{
    [Authorize]
    [Route("api/public/settings")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IUserModelBuilder _modelBuilder;
        private readonly ISender _sender;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(IContextService contextService, 
            IUserModelBuilder modelBuilder,
            ISender sender, 
            ILogger<SettingsController> logger)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _sender = sender;
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

            await _sender.Send(command);

            return Ok();
        }

        [HttpGet("is-display-name-unique/{name}")]
        public async Task<IActionResult> IsDisplayNameUnique(string name)
        {
            var isNameUnique = await _sender.Send(new IsUserDisplayNameUnique { DisplayName = name });
            return Ok(isNameUnique);
        }
    }
}
