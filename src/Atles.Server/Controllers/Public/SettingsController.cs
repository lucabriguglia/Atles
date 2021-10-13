using System.Threading.Tasks;
using Atles.Domain.Categories.Rules;
using Atles.Domain.Users.Commands;
using Atles.Models.Public.Users;
using Atles.Reporting.Public.Queries;
using Atles.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenCqrs;

namespace Atles.Server.Controllers.Public
{
    [Authorize]
    [Route("api/public/settings")]
    public class SettingsController : SiteControllerBase
    {
        private readonly IContextService _contextService;
        private readonly ISender _sender;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(IContextService contextService, 
            ISender sender, 
            ILogger<SettingsController> logger) : base(sender)
        {
            _contextService = contextService;
            _sender = sender;
            _logger = logger;
        }

        [HttpGet("edit")]
        public async Task<ActionResult<SettingsPageModel>> Edit()
        {
            var site = await CurrentSite();
            var user = await _contextService.CurrentUserAsync();

            var model = await _sender.Send(new GetSettingsPage { SiteId = site.Id, UserId = user.Id });

            return model;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(SettingsPageModel model)
        {
            var site = await CurrentSite();
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
