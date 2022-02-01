using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Commands;
using Atles.Domain.Models;
using Atles.Infrastructure;
using Atles.Reporting.Models.Public;
using Atles.Reporting.Models.Public.Queries;
using Atles.Reporting.Models.Shared;
using Atles.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atles.Server.Controllers.Public
{
    [Route("api/public/topics")]
    public class TopicsController : SiteControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly AtlesDbContext _dbContext;
        private readonly ILogger<TopicsController> _logger;
        private readonly IDispatcher _dispatcher;

        public TopicsController(ISecurityService securityService, 
            AtlesDbContext dbContext, 
            ILogger<TopicsController> logger,
            IDispatcher dispatcher) : base(dispatcher)
        {
            _securityService = securityService;
            _dbContext = dbContext;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        [HttpGet("{forumSlug}/{topicSlug}")]
        public async Task<ActionResult<TopicPageModel>> Topic(string forumSlug, string topicSlug, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var model = await _dispatcher.Get(new GetTopicPage 
            { 
                SiteId = site.Id, 
                ForumSlug = forumSlug, 
                TopicSlug = topicSlug, 
                Options = new QueryOptions(page, search) 
            });

            if (model == null)
            {
                _logger.LogWarning("Topic not found.");
                return NotFound();
            }

            var permissions = await _dispatcher.Get(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = model.Forum.Id 
            });

            var canRead = _securityService.HasPermission(PermissionType.Read, permissions);

            if (!canRead)
            {
                _logger.LogWarning("Unauthorized access to topic");
                return Unauthorized();
            }

            model.CanEdit = _securityService.HasPermission(PermissionType.Edit, permissions) && !user.IsSuspended;
            model.CanReply = _securityService.HasPermission(PermissionType.Reply, permissions) && !user.IsSuspended;
            model.CanDelete = _securityService.HasPermission(PermissionType.Delete, permissions) && !user.IsSuspended;
            model.CanModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !user.IsSuspended;
            model.CanReact = _securityService.HasPermission(PermissionType.Reactions, permissions) && !user.IsSuspended;

            return model;
        }

        [HttpGet("{forumId}/{topicId}/replies")]
        public async Task<ActionResult<PaginatedData<TopicPageModel.ReplyModel>>> Replies(Guid forumId, Guid topicId, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await CurrentSite();

            var permissions = await _dispatcher.Get(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = forumId 
            });

            var canRead = _securityService.HasPermission(PermissionType.Read, permissions);

            if (!canRead)
            {
                _logger.LogWarning("Unauthorized access to topic replies");
                return Unauthorized();
            }

            var result = await _dispatcher.Get(new GetTopicPageReplies
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

            var model = await _dispatcher.Get(new GetCreatePostPage { SiteId = site.Id, ForumId = forumId });

            if (model == null)
            {
                _logger.LogWarning("Forum for new topic not found.");
                return NotFound();
            }

            var permissions = await _dispatcher.Get(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = model.Forum.Id 
            });

            var canPost = _securityService.HasPermission(PermissionType.Start, permissions) && !user.IsSuspended;

            if (!canPost)
            {
                _logger.LogWarning("Unauthorized access to new topic");
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

            var model = await _dispatcher.Get(new GetEditPostPage { SiteId = site.Id, ForumId = forumId, TopicId = topicId });

            if (model == null)
            {
                _logger.LogWarning("Topic to edit not found.");
                return NotFound();
            }

            var permissions = await _dispatcher.Get(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = forumId 
            });

            var canEdit = _securityService.HasPermission(PermissionType.Edit, permissions);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
            var authorized = (canEdit && model.Topic.UserId == user.Id && !model.Topic.Locked || canModerate) && !user.IsSuspended;

            if (!authorized)
            {
                _logger.LogWarning("Unauthorized access to edit topic");
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

            var permissions = await _dispatcher.Get(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = model.Forum.Id 
            });

            var canPost = _securityService.HasPermission(PermissionType.Start, permissions) && !user.IsSuspended;

            if (!canPost)
            {
                _logger.LogWarning("Unauthorized access to create topic");
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

            await _dispatcher.Send(command);

            var slug = await _dispatcher.Get(new GetTopicSlug
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

            var permissions = await _dispatcher.Get(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = model.Forum.Id 
            });

            var canEdit = _securityService.HasPermission(PermissionType.Edit, permissions);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
            var authorized = (canEdit && topicInfo.UserId == user.Id && !topicInfo.Locked || canModerate) && !user.IsSuspended;

            if (!authorized)
            {
                _logger.LogWarning("Unauthorized access to update topic");
                return Unauthorized();
            }

            await _dispatcher.Send(command);

            var slug = await _dispatcher.Get(new GetTopicSlug
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

            var permissions = await _dispatcher.Get(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = forumId 
            });

            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !user.IsSuspended;

            if (!canModerate)
            {
                _logger.LogWarning("Unauthorized access to pin topic");
                return Unauthorized();
            }

            await _dispatcher.Send(command);

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

            var permissions = await _dispatcher.Get(new GetPermissions 
            { 
                SiteId = site.Id, 
                ForumId = forumId 
            });

            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !user.IsSuspended;

            if (!canModerate)
            {
                _logger.LogWarning("Unauthorized access to lock topic");
                return Unauthorized();
            }

            await _dispatcher.Send(command);

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

            var permissions = await _dispatcher.Get(new GetPermissions
            {
                SiteId = site.Id,
                ForumId = forumId
            });

            var canDelete = _securityService.HasPermission(PermissionType.Delete, permissions);
            var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
            var authorized = (canDelete && topicUserId == user.Id || canModerate) && !user.IsSuspended;

            if (!authorized)
            {
                _logger.LogWarning("Unauthorized access to delete topic");
                return Unauthorized();
            }

            await _dispatcher.Send(command);

            return Ok();
        }
    }
}
