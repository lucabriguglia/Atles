using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.PermissionSets;
using Atles.Domain.Posts;
using Atles.Domain.Posts.Commands;
using Atles.Domain.Posts.Generators;
using Atles.Models;
using Atles.Models.Public.Posts;
using Atles.Models.Public.Topics;
using Atles.Reporting.Public.Queries;
using Atles.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenCqrs;

namespace Atles.Server.Controllers.Public
{
    [Route("api/public/topics")]
    public class TopicsController : SiteControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly AtlesDbContext _dbContext;
        private readonly ILogger<TopicsController> _logger;
        private readonly ISender _sender;

        public TopicsController(ISecurityService securityService, 
            AtlesDbContext dbContext, 
            ILogger<TopicsController> logger,
            ISender sender) : base(sender)
        {
            _securityService = securityService;
            _dbContext = dbContext;
            _logger = logger;
            _sender = sender;
        }

        [HttpGet("{forumSlug}/{topicSlug}")]
        public async Task<ActionResult<TopicPageModel>> Topic(string forumSlug, string topicSlug, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var model = await _sender.Send(new GetTopicPage 
            { 
                SiteId = site.Id, 
                ForumSlug = forumSlug, 
                TopicSlug = topicSlug, 
                Options = new QueryOptions(page, search) 
            });

            if (model == null)
            {
                _logger.LogWarning("Topic not found.", new
                {
                    SiteId = site.Id,
                    ForumSlug = forumSlug,
                    TopicSlug = topicSlug,
                    User = User.Identity.Name
                });

                return NotFound();
            }

            var permissions = await _sender.Send(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = model.Forum.Id 
            });

            var canRead = _securityService.HasPermission(PermissionType.Read, permissions);

            if (!canRead)
            {
                _logger.LogWarning("Unauthorized access to topic", new
                {
                    SiteId = site.Id,
                    ForumSlug = forumSlug,
                    TopicSlug = topicSlug,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            model.CanEdit = _securityService.HasPermission(PermissionType.Edit, permissions) && !user.IsSuspended;
            model.CanReply = _securityService.HasPermission(PermissionType.Reply, permissions) && !user.IsSuspended;
            model.CanDelete = _securityService.HasPermission(PermissionType.Delete, permissions) && !user.IsSuspended;
            model.CanModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !user.IsSuspended;

            return model;
        }

        [HttpGet("{forumId}/{topicId}/replies")]
        public async Task<ActionResult<PaginatedData<TopicPageModel.ReplyModel>>> Replies(Guid forumId, Guid topicId, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await CurrentSite();

            var permissions = await _sender.Send(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = forumId 
            });

            var canRead = _securityService.HasPermission(PermissionType.Read, permissions);

            if (!canRead)
            {
                _logger.LogWarning("Unauthorized access to topic replies", new
                {
                    SiteId = site.Id,
                    ForumId = forumId,
                    TopicId = topicId,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            var result = await _sender.Send(new GetTopicPageReplies
            {
                SiteId = site.Id,
                TopicId = topicId,
                Options = new QueryOptions(page, search)
            });

            return result;
        }

        [Authorize]
        [HttpGet("{forumId}/new-topic")]
        public async Task<ActionResult<PostPageModel>> NewTopic(Guid forumId)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var model = await _sender.Send(new GetCreatePostPage { SiteId = site.Id, ForumId = forumId });

            if (model == null)
            {
                _logger.LogWarning("Forum for new topic not found.", new
                {
                    SiteId = site.Id,
                    ForumId = forumId,
                    User = User.Identity.Name
                });

                return NotFound();
            }

            var permissions = await _sender.Send(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = model.Forum.Id 
            });

            var canPost = _securityService.HasPermission(PermissionType.Start, permissions) && !user.IsSuspended;

            if (!canPost)
            {
                _logger.LogWarning("Unauthorized access to new topic", new
                {
                    SiteId = site.Id,
                    ForumId = forumId,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            return model;
        }

        [Authorize]
        [HttpGet("{forumId}/edit-topic/{topicId}")]
        public async Task<ActionResult<PostPageModel>> EditTopic(Guid forumId, Guid topicId)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var model = await _sender.Send(new GetEditPostPage { SiteId = site.Id, ForumId = forumId, TopicId = topicId });

            if (model == null)
            {
                _logger.LogWarning("Topic to edit not found.", new
                {
                    SiteId = site.Id,
                    ForumId = forumId,
                    TopicId = topicId,
                    User = User.Identity.Name
                });

                return NotFound();
            }

            var permissions = await _sender.Send(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = forumId 
            });

            var canEdit = _securityService.HasPermission(PermissionType.Edit, permissions);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
            var authorized = (canEdit && model.Topic.UserId == user.Id && !model.Topic.Locked || canModerate) && !user.IsSuspended;

            if (!authorized)
            {
                _logger.LogWarning("Unauthorized access to edit topic", new
                {
                    SiteId = site.Id,
                    ForumId = forumId,
                    TopicId = topicId,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            return model;
        }

        [Authorize]
        [HttpPost("create-topic")]
        public async Task<ActionResult> CreateTopic(PostPageModel model)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var permissions = await _sender.Send(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = model.Forum.Id 
            });

            var canPost = _securityService.HasPermission(PermissionType.Start, permissions) && !user.IsSuspended;

            if (!canPost)
            {
                _logger.LogWarning("Unauthorized access to create topic", new
                {
                    SiteId = site.Id,
                    ForumId = model.Forum?.Id,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            var command = new CreateTopic
            {
                ForumId = model.Forum.Id,
                Title = model.Topic.Title,
                Content = model.Topic.Content,
                Status = PostStatusType.Published,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _sender.Send(command);

            var slug = await _sender.Send(new GetTopicSlug
            {
                TopicId = command.Id
            });

            return Ok(slug);
        }

        [Authorize]
        [HttpPost("update-topic")]
        public async Task<ActionResult> UpdateTopic(PostPageModel model)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new UpdateTopic
            {
                Id = model.Topic.Id,
                ForumId = model.Forum.Id,
                Title = model.Topic.Title,
                Content = model.Topic.Content,
                Status = PostStatusType.Published,
                SiteId = site.Id,
                UserId = user.Id
            };

            var topicInfo = await _dbContext.Posts
                .Where(x =>
                    x.Id == command.Id &&
                    x.TopicId == null &&
                    x.ForumId == command.ForumId &&
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Status != PostStatusType.Deleted)
                .Select(x => new { UserId = x.CreatedBy, x.Locked})
                .FirstOrDefaultAsync();

            var permissions = await _sender.Send(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = model.Forum.Id 
            });

            var canEdit = _securityService.HasPermission(PermissionType.Edit, permissions);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
            var authorized = (canEdit && topicInfo.UserId == user.Id && !topicInfo.Locked || canModerate) && !user.IsSuspended;

            if (!authorized)
            {
                _logger.LogWarning("Unauthorized access to update topic", new
                {
                    SiteId = site.Id,
                    ForumId = model.Forum?.Id,
                    TopicId = model.Topic?.Id,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            await _sender.Send(command);

            var slug = await _sender.Send(new GetTopicSlug
            {
                TopicId = command.Id
            });

            return Ok(slug);
        }

        [Authorize]
        [HttpPost("pin-topic/{forumId}/{topicId}")]
        public async Task<ActionResult> PinTopic(Guid forumId, Guid topicId, [FromBody] bool pinned)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new PinTopic
            {
                Id = topicId,
                ForumId = forumId,
                Pinned = pinned,
                SiteId = site.Id,
                UserId = user.Id
            };

            var permissions = await _sender.Send(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = forumId 
            });

            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !user.IsSuspended;

            if (!canModerate)
            {
                _logger.LogWarning("Unauthorized access to pin topic", new
                {
                    SiteId = site.Id,
                    ForumId = forumId,
                    TopicId = topicId,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            await _sender.Send(command);

            return Ok();
        }

        [Authorize]
        [HttpPost("lock-topic/{forumId}/{topicId}")]
        public async Task<ActionResult> LockTopic(Guid forumId, Guid topicId, [FromBody] bool locked)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new LockTopic
            {
                Id = topicId,
                ForumId = forumId,
                Locked = locked,
                SiteId = site.Id,
                UserId = user.Id
            };

            var permissions = await _sender.Send(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = forumId 
            });

            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !user.IsSuspended;

            if (!canModerate)
            {
                _logger.LogWarning("Unauthorized access to lock topic", new
                {
                    SiteId = site.Id,
                    ForumId = forumId,
                    TopicId = topicId,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            await _sender.Send(command);

            return Ok();
        }

        [Authorize]
        [HttpDelete("delete-topic/{forumId}/{topicId}")]
        public async Task<ActionResult> DeleteTopic(Guid forumId, Guid topicId)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new DeleteTopic
            {
                Id = topicId,
                ForumId = forumId,
                SiteId = site.Id,
                UserId = user.Id
            };

            var topicUserId = await _dbContext.Posts
                .Where(x =>
                    x.Id == command.Id &&
                    x.TopicId == null &&
                    x.ForumId == command.ForumId &&
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Status != PostStatusType.Deleted)
                .Select(x => x.CreatedBy)
                .FirstOrDefaultAsync();

            var permissions = await _sender.Send(new GetPermissions
            {
                SiteId = site.Id,
                ForumId = forumId
            });

            var canDelete = _securityService.HasPermission(PermissionType.Delete, permissions);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
            var authorized = (canDelete && topicUserId == user.Id || canModerate) && !user.IsSuspended;

            if (!authorized)
            {
                _logger.LogWarning("Unauthorized access to delete topic", new
                {
                    SiteId = site.Id,
                    ForumId = forumId,
                    TopicId = topicId,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            await _sender.Send(command);

            return Ok();
        }
    }
}
