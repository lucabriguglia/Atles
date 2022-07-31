using System.Threading.Tasks;
using Atles.Commands.Sites;
using Atles.Core;
using Atles.Models.Admin.Sites;
using Atles.Queries.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/sites")]
public class SitesController : AdminControllerBase
{
    private readonly IDispatcher _dispatcher;

    public SitesController(IDispatcher dispatcher) : base(dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("settings")]
    public async Task<ActionResult<SettingsPageModel>> Settings()
    {
        return await ProcessGet(new GetSettingsPageModel
        {
            SiteId = CurrentSite.Id
        });
    }

    [HttpPost("update")]
    public async Task<ActionResult> Update(SettingsPageModel model)
    {
        var command = new UpdateSite
        {
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id,
            Title = model.Site.Title,
            Theme = model.Site.Theme,
            Css = model.Site.Css,
            Language = model.Site.Language,
            Privacy = model.Site.Privacy,
            Terms = model.Site.Terms,
            HeadScript = model.Site.HeadScript
        };

        await _dispatcher.Send(command);

        return Ok();
    }
}
