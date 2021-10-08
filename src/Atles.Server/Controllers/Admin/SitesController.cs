using System.Threading.Tasks;
using Atles.Domain.Sites.Commands;
using Atles.Models.Admin.Sites;
using Atles.Reporting.Admin.Sites.Queries;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;
using OpenCqrs.Commands;
using OpenCqrs.Queries;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/sites")]
    public class SitesController : AdminControllerBase
    {
        private readonly IContextService _contextService;
        private readonly ICommandSender _commandSender;
        private readonly IQuerySender _querySender;

        public SitesController(IContextService contextService,
            ICommandSender commandSender,
            IQuerySender querySender)
        {
            _contextService = contextService;
            _commandSender = commandSender;
            _querySender = querySender;
        }

        [HttpGet("settings")]
        public async Task<ActionResult<SettingsPageModel>> Settings()
        {
            var site = await _contextService.CurrentSiteAsync();

            var result = await _querySender.Send(new GetSettingsPageModel { SiteId = site.Id });

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(SettingsPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new UpdateSite
            {
                SiteId = site.Id,
                UserId = user.Id,
                Title = model.Site.Title,
                Theme = model.Site.Theme,
                Css = model.Site.Css,
                Language = model.Site.Language,
                Privacy = model.Site.Privacy,
                Terms = model.Site.Terms,
                HeadScript = model.Site.HeadScript
            };

            await _commandSender.Send(command);

            return Ok();
        }
    }
}
