using System;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Domain.Replies;
using Atlas.Domain.Replies.Commands;
using Atlas.Domain.Topics;
using Atlas.Domain.Topics.Commands;
using Atlas.Models;
using Atlas.Models.Public;
using Atlas.Server.Services;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers
{
    [Route("api/public")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IPublicModelBuilder _modelBuilder;
        private readonly ITopicService _topicService;
        private readonly IReplyService _replyService;

        public PublicController(IContextService contextService, 
            IPublicModelBuilder modelBuilder, 
            ITopicService topicService, 
            IReplyService replyService)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _topicService = topicService;
            _replyService = replyService;
        }

        [HttpGet("index-model")]
        public async Task<IndexPageModel> Index()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildIndexPageModelAsync(site.Id);
        }

        [HttpGet("forum/{id}")]
        public async Task<ActionResult<ForumPageModel>> Forum(Guid id, [FromQuery] int? page = 1)
        {
            var site = await _contextService.CurrentSiteAsync();

            var model = await _modelBuilder.BuildForumPageModelAsync(site.Id, id, new PaginationOptions(page));

            if (model == null)
            {
                return NotFound();
            }



            return model;
        }

        [HttpGet("forum/{forumId}/{topicId}")]
        public async Task<ActionResult<TopicPageModel>> Topic(Guid forumId, Guid topicId, [FromQuery] int? page = 1)
        {
            var site = await _contextService.CurrentSiteAsync();

            var model = await _modelBuilder.BuildTopicPageModelAsync(site.Id, forumId, topicId, new PaginationOptions(page));

            if (model == null)
            {
                return NotFound();
            }

            return model;
        }

        [Authorize]
        [HttpGet("forum/{forumId}/post")]
        public async Task<ActionResult<PostPageModel>> Post(Guid forumId)
        {
            var site = await _contextService.CurrentSiteAsync();

            var model = await _modelBuilder.BuildPostPageModelAsync(site.Id, forumId);

            if (model == null)
            {
                return NotFound();
            }

            return model;
        }

        [Authorize]
        [HttpPost("save-topic")]
        public async Task<ActionResult> SaveTopic(PostPageModel model)
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

        [Authorize]
        [HttpPost("save-reply")]
        public async Task<ActionResult> SaveReply(TopicPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new CreateReply
            {
                ForumId = model.Forum.Id,
                TopicId = model.Topic.Id,
                Content = model.Post.Content,
                Status = StatusType.Published,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _replyService.CreateAsync(command);

            return Ok();
        }

        [Authorize]
        [HttpPost("preview")]
        public async Task<string> Preview([FromBody]string content)
        {
            return await Task.FromResult(Markdown.ToHtml(content));
        }
    }
}
