using System.Threading.Tasks;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Atlas.Domain.Sites.Commands;
using Atlas.Domain.Sites;
using Atlas.Models.Admin.Site;

namespace Atlas.Server.Controllers.Admin
{
    [Route("api/admin/sites")]
    public class SitesController : AdminControllerBase
    {
        private readonly IContextService _contextService;
        private readonly ISiteService _siteService;
        private readonly ISiteModelBuilder _modelBuilder;

        public SitesController(IContextService contextService,
            ISiteService siteService,
            ISiteModelBuilder modelBuilder)
        {
            _contextService = contextService;
            _siteService = siteService;
            _modelBuilder = modelBuilder;
        }

        [HttpGet("settings")]
        public async Task<ActionResult<SettingsPageModel>> Settings()
        {
            var site = await _contextService.CurrentSiteAsync();

            var result = await _modelBuilder.BuildSettingsPageModelAsync(site.Id);

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

            await _siteService.UpdateAsync(command);

            return Ok();
        }
    }
}
