using System;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Domain.Topics;
using Atlas.Domain.Topics.Commands;
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
        private readonly ITopicService _topicService;

        public IndexController(IContextService contextService, IPublicModelBuilder modelBuilder, ITopicService topicService)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _topicService = topicService;
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

        [HttpGet("forum/{forumId}/{topicId}")]
        public async Task<ActionResult<TopicPageModel>> Topic(Guid forumId, Guid topicId)
        {
            var site = await _contextService.CurrentSiteAsync();

            var model = await _modelBuilder.BuildTopicPageModelAsync(site.Id, forumId, topicId);

            if (model == null)
            {
                return NotFound();
            }

            return model;
        }

        [HttpGet("forum/{forumId}/new-topic")]
        public async Task<ActionResult<PostPageModel>> NewTopic(Guid forumId)
        {
            var site = await _contextService.CurrentSiteAsync();

            var model = await _modelBuilder.BuildPostPageModelAsync(site.Id, forumId);

            if (model == null)
            {
                return NotFound();
            }

            return model;
        }

        [HttpPost("save-topic")]
        public async Task<ActionResult> Save(PostPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new CreateTopic
            {
                ForumId = model.Forum.Id,
                Title = model.Topic.Title,
                Content = model.Topic.Content,
                Status = StatusType.Published,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _topicService.CreateAsync(command);

            return Ok();
        }
    }
}
