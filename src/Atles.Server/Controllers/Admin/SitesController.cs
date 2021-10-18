using System.Threading.Tasks;
using Atles.Domain.Sites.Commands;
using Atles.Models.Admin.Sites;
using Atles.Reporting.Admin.Sites.Queries;
using Microsoft.AspNetCore.Mvc;
using OpenCqrs;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/sites")]
    public class SitesController : AdminControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public SitesController(IDispatcher sender) : base(sender)
        {
            _dispatcher = sender;
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
