using System;
using System.Threading.Tasks;
using Atlas.Server.Services;
using Atlas.Shared.Public;
using Atlas.Shared.Public.Models;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers
{
    [Route("api/index")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IPublicModelBuilder _modelBuilder;

        public IndexController(IContextService contextService, IPublicModelBuilder modelBuilder)
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

        [HttpGet("forum/{forumId}/new-topic")]
        public async Task<ActionResult<TopicPageModel>> NewTopic(Guid forumId)
        {
            var site = await _contextService.CurrentSiteAsync();

            var model = await _modelBuilder.BuildNewTopicPageModelAsync(site.Id, forumId);

            if (model == null)
            {
                return NotFound();
            }

            return model;
        }
    }
}
