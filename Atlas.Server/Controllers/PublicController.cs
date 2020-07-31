using System;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Domain.PermissionSets;
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
        private readonly ISecurityService _securityService;

        public PublicController(IContextService contextService, 
            IPublicModelBuilder modelBuilder, 
            ITopicService topicService, 
            IReplyService replyService, 
            ISecurityService securityService)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
            _topicService = topicService;
            _replyService = replyService;
            _securityService = securityService;
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
        [HttpGet("forum/{forumId}/new-topic")]
        public async Task<ActionResult<PostPageModel>> NewTopic(Guid forumId)
        {
            var site = await _contextService.CurrentSiteAsync();

            var model = await _modelBuilder.BuildNewPostPageModelAsync(site.Id, forumId);

            if (model == null)
            {
                return NotFound();
            }

            var canPost = await _securityService.HasPermission(PermissionType.Start, site.Id, model.Forum.Id);

            if (!canPost)
            {
                return Unauthorized();
            }

            return model;
        }

        [Authorize]
        [HttpGet("forum/{forumId}/edit-topic/{topicId}")]
        public async Task<ActionResult<PostPageModel>> EditTopic(Guid forumId, Guid topicId)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var model = await _modelBuilder.BuildEditPostPageModelAsync(site.Id, forumId, topicId);

            if (model == null)
            {
                return NotFound();
            }

            var canEdit = await _securityService.HasPermission(PermissionType.Edit, site.Id, model.Forum.Id);
            var authorized = canEdit && model.Topic.MemberId == member.Id || User.IsInRole(Consts.RoleNameAdmin);

            if (!authorized)
            {
                return Unauthorized();
            }

            return model;
        }

        [Authorize]
        [HttpPost("create-topic")]
        public async Task<ActionResult> CreateTopic(PostPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var canPost = await _securityService.HasPermission(PermissionType.Start, site.Id, model.Forum.Id);

            if (!canPost)
            {
                return Unauthorized();
            }

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
        [HttpPost("update-topic")]
        public async Task<ActionResult> UpdateTopic(PostPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var canEdit = await _securityService.HasPermission(PermissionType.Edit, site.Id, model.Forum.Id);
            var authorized = canEdit && model.Topic.MemberId == member.Id || User.IsInRole(Consts.RoleNameAdmin);

            if (!authorized)
            {
                return Unauthorized();
            }

            var command = new UpdateTopic
            {
                Id = model.Topic.Id,
                ForumId = model.Forum.Id,
                Title = model.Topic.Title,
                Content = model.Topic.Content,
                Status = StatusType.Published,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _topicService.UpdateAsync(command);

            return Ok();
        }

        [HttpDelete("delete/{forumId}/{topicId}/{userId}")]
        public async Task<ActionResult> DeleteTopic(Guid forumId, Guid topicId, string userId)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var canDelete = await _securityService.HasPermission(PermissionType.Delete, site.Id, forumId);
            var authorized = canDelete && userId == member.UserId || User.IsInRole(Consts.RoleNameAdmin);

            if (!authorized)
            {
                return Unauthorized();
            }

            var command = new DeleteTopic
            {
                Id = topicId,
                ForumId = forumId,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _topicService.DeleteAsync(command);

            return Ok();
        }

        [Authorize]
        [HttpPost("create-reply")]
        public async Task<ActionResult> CreateReply(TopicPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var canReply = await _securityService.HasPermission(PermissionType.Reply, site.Id, model.Forum.Id);

            if (!canReply)
            {
                return Unauthorized();
            }

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
        [HttpPost("update-reply")]
        public async Task<ActionResult> UpdateReply(TopicPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var canEdit = await _securityService.HasPermission(PermissionType.Edit, site.Id, model.Forum.Id);
            var authorized = canEdit && model.Post.MemberId == member.Id || User.IsInRole(Consts.RoleNameAdmin);

            if (!authorized)
            {
                return Unauthorized();
            }

            var command = new UpdateReply
            {
                Id = model.Post.Id.Value,
                ForumId = model.Forum.Id,
                TopicId = model.Topic.Id,
                Content = model.Post.Content,
                Status = StatusType.Published,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _replyService.UpdateAsync(command);

            return Ok();
        }

        [HttpDelete("delete/{forumId}/{topicId}/{replyId}/{userId}")]
        public async Task<ActionResult> DeleteReply(Guid forumId, Guid topicId, Guid replyId, string userId)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var canDelete = await _securityService.HasPermission(PermissionType.Delete, site.Id, forumId);
            var authorized = canDelete && userId == member.UserId || User.IsInRole(Consts.RoleNameAdmin);

            if (!authorized)
            {
                return Unauthorized();
            }

            var command = new DeleteReply
            {
                Id = replyId,
                TopicId = topicId,
                ForumId = forumId,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _replyService.DeleteAsync(command);

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
