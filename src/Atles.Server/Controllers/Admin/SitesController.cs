using System.Threading.Tasks;
using Atles.Domain.Models.Sites.Commands;
using Atles.Infrastructure;
using Atles.Reporting.Models.Admin.Sites;
using Atles.Reporting.Models.Admin.Sites.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin
{
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
            var site = await CurrentSite();

            var result = await _dispatcher.Get(new GetSettingsPageModel { SiteId = site.Id });

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(SettingsPageModel model)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

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

            await _dispatcher.Send(command);

            return Ok();
        }
    }
}
