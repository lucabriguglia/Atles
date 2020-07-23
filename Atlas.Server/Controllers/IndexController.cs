using System.Threading.Tasks;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Atlas.Shared.Site;
using Atlas.Shared.Site.Models;
using System;

namespace Atlas.Server.Controllers.Admin
{
    [Route("api/index")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly ISiteModelBuilder _modelBuilder;

        public IndexController(IContextService contextService, ISiteModelBuilder modelBuilder)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
        }

        [HttpGet("index-model")]
        public async Task<IndexPageModel> Index()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildIndexPageModelAsync(site.Id);
        }

        [HttpGet("forum/{id}")]
        public async Task<ActionResult<ForumPageModel>> Forum(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();

            var model = await _modelBuilder.BuildForumPageModelAsync(site.Id, id);

            if (model == null)
            {
                return NotFound();
            }

            return model;
        }
    }
}
