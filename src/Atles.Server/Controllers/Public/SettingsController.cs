using Atles.Commands.Users;
using Atles.Core;
using Atles.Models.Public;
using Atles.Queries.Public;
using Atles.Validators.ValidationRules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Public;

[Authorize]
[Route("api/public/settings")]
public class SettingsController : SiteControllerBase
{
    private readonly IDispatcher _dispatcher;
    private readonly ILogger<SettingsController> _logger;
    private readonly IUserValidationRules _userValidationRules;

    public SettingsController(
        IDispatcher dispatcher, 
        ILogger<SettingsController> logger, 
        IUserValidationRules userValidationRules) 
        : base(dispatcher)
    {
        _dispatcher = dispatcher;
        _logger = logger;
        _userValidationRules = userValidationRules;
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
            Id = CurrentUser.Id,
            DisplayName = model.User.DisplayName,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpGet("is-display-name-unique/{displayName}")]
    public async Task<IActionResult> IsDisplayNameUnique(string displayName) => 
        Ok(await _userValidationRules.IsUserDisplayNameUnique(CurrentUser.Id, displayName));
}
