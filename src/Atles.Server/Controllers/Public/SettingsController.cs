using System.Threading.Tasks;
using Atles.Commands.Users;
using Atles.Core;
using Atles.Domain.Rules.Users;
using Atles.Models.Public;
using Atles.Queries.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atles.Server.Controllers.Public;

[Authorize]
[Route("api/public/settings")]
public class SettingsController : SiteControllerBase
{
    private readonly IDispatcher _dispatcher;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(IDispatcher dispatcher, ILogger<SettingsController> logger) : base(dispatcher)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    [HttpGet("edit")]
    public async Task<ActionResult<SettingsPageModel>> Edit()
    {
        return await ProcessGet(new GetSettingsPage 
        { 
            SiteId = CurrentSite.Id, 
            UserId = CurrentUser.Id
        });
    }

    [HttpPost("update")]
    public async Task<ActionResult> Update(SettingsPageModel model)
    {
        if (model.User.Id != CurrentUser.Id || CurrentUser.IsSuspended)
        {
            _logger.LogWarning("Unauthorized access to update settings.");
            return Unauthorized();
        }

        var command = new UpdateUser
        {
            UpdateUserId = CurrentUser.Id,
            DisplayName = model.User.DisplayName,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpGet("is-display-name-unique/{name}")]
    public async Task<IActionResult> IsDisplayNameUnique(string name)
    {
        return await ProcessGet(new IsUserDisplayNameUnique
        {
            DisplayName = name
        });
    }
}
