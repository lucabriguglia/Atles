using Atles.Commands.Posts;
using Atles.Core;
using Atles.Data;
using Atles.Domain;
using Atles.Models;
using Atles.Models.Public;
using Atles.Queries.Public;
using Atles.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server.Controllers.Public;

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
        var getTopicPageResult = await _dispatcher.Get(new GetTopicPage 
        { 
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id,
            ForumSlug = forumSlug,
            TopicSlug = topicSlug, 
            Options = new QueryOptions(page, search) 
        });

        var model = getTopicPageResult.AsT0;

        if (model == null)
        {
            _logger.LogWarning("Topic not found.");
            return NotFound();
        }

        var getPermissionsResult = await _dispatcher.Get(new GetPermissions 
        { 
            SiteId = CurrentSite.Id, 
            ForumId = model.Forum.Id 
        });

        var permissions = getPermissionsResult.AsT0;

        var canRead = _securityService.HasPermission(PermissionType.Read, permissions);

        if (!canRead)
        {
            _logger.LogWarning("Unauthorized access to topic");
            return Unauthorized();
        }

        model.Permissions.CanEdit = _securityService.HasPermission(PermissionType.Edit, permissions) && !CurrentUser.IsSuspended;
        model.Permissions.CanReply = _securityService.HasPermission(PermissionType.Reply, permissions) && !CurrentUser.IsSuspended;
        model.Permissions.CanDelete = _securityService.HasPermission(PermissionType.Delete, permissions) && !CurrentUser.IsSuspended;
        model.Permissions.CanModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !CurrentUser.IsSuspended;
        model.Permissions.CanReact = _securityService.HasPermission(PermissionType.Reactions, permissions) && !CurrentUser.IsSuspended;

        return model;
    }

    [HttpGet("{forumId}/{topicId}/replies")]
    public async Task<ActionResult<PaginatedData<TopicPageModel.ReplyModel>>> Replies(Guid forumId, Guid topicId, [FromQuery] int? page = 1, [FromQuery] string search = null)
    {
        var getPermissionsResult = await _dispatcher.Get(new GetPermissions 
        {
            SiteId = CurrentSite.Id,
            ForumId = forumId 
        });

        var permissions = getPermissionsResult.AsT0;

        var canRead = _securityService.HasPermission(PermissionType.Read, permissions);

        if (!canRead)
        {
            _logger.LogWarning("Unauthorized access to topic replies");
            return Unauthorized();
        }

        var result = await _dispatcher.Get(new GetTopicPageReplies
        {
            SiteId = CurrentSite.Id,
            TopicId = topicId,
            Options = new QueryOptions(page, search)
        });

        return result.AsT0;
    }

    [Authorize]
    [HttpGet("{forumId}/new-topic")]
    public async Task<ActionResult<PostPageModel>> NewTopic(Guid forumId)
    {
        var getCreatePostPageResult = await _dispatcher.Get(new GetCreatePostPage { SiteId = CurrentSite.Id, ForumId = forumId });

        var model = getCreatePostPageResult.AsT0;

        if (model == null)
        {
            _logger.LogWarning("Forum for new topic not found.");
            return NotFound();
        }

        var getPermissionsResult = await _dispatcher.Get(new GetPermissions 
        { 
            SiteId = CurrentSite.Id, 
            ForumId = model.Forum.Id 
        });

        var permissions = getPermissionsResult.AsT0;

        var canPost = _securityService.HasPermission(PermissionType.Start, permissions) && !CurrentUser.IsSuspended;

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
        var getEditPostPageResult = await _dispatcher.Get(new GetEditPostPage
        {
            SiteId = CurrentSite.Id, 
            ForumId = forumId, 
            TopicId = topicId, 
            UserId = CurrentUser.Id
        });

        var model = getEditPostPageResult.AsT0;

        if (model == null)
        {
            _logger.LogWarning("Topic to edit not found.");
            return NotFound();
        }

        var getPermissionsResult = await _dispatcher.Get(new GetPermissions 
        { 
            SiteId = CurrentSite.Id, 
            ForumId = forumId 
        });

        var permissions = getPermissionsResult.AsT0;

        var canEdit = _securityService.HasPermission(PermissionType.Edit, permissions);
        var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
        var authorized = (canEdit && model.Topic.UserId == CurrentUser.Id && !model.Topic.Locked || canModerate) && !CurrentUser.IsSuspended;

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
        var getPermissionsResult = await _dispatcher.Get(new GetPermissions 
        { 
            SiteId = CurrentSite.Id, 
            ForumId = model.Forum.Id 
        });

        var permissions = getPermissionsResult.AsT0;

        var canPost = _securityService.HasPermission(PermissionType.Start, permissions) && !CurrentUser.IsSuspended;

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
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id,
            Subscribe = model.Topic.Subscribe
        };

        await _dispatcher.Send(command);

        var slug = await _dispatcher.Get(new GetTopicSlug
        {
            TopicId = command.TopicId
        });

        return Ok(slug.AsT0);
    }

    [Authorize]
    [HttpPost("update-topic")]
    public async Task<ActionResult> UpdateTopic(PostPageModel model)
    {
        var command = new UpdateTopic
        {
            TopicId = model.Topic.Id,
            ForumId = model.Forum.Id,
            Title = model.Topic.Title,
            Content = model.Topic.Content,
            Status = PostStatusType.Published,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id,
            Subscribe = model.Topic.Subscribe
        };

        var topicInfo = await _dbContext.Posts
            .Where(x =>
                x.Id == command.TopicId &&
                x.TopicId == null &&
                x.ForumId == command.ForumId &&
                x.Forum.Category.SiteId == command.SiteId &&
                x.Status != PostStatusType.Deleted)
            .Select(x => new { UserId = x.CreatedBy, x.Locked})
            .FirstOrDefaultAsync();

        var getPermissionsResult = await _dispatcher.Get(new GetPermissions 
        { 
            SiteId = CurrentSite.Id, 
            ForumId = model.Forum.Id 
        });

        var permissions = getPermissionsResult.AsT0;

        var canEdit = _securityService.HasPermission(PermissionType.Edit, permissions);
        var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
        var authorized = (canEdit && topicInfo.UserId == CurrentUser.Id && !topicInfo.Locked || canModerate) && !CurrentUser.IsSuspended;

        if (!authorized)
        {
            _logger.LogWarning("Unauthorized access to update topic");
            return Unauthorized();
        }

        await _dispatcher.Send(command);

        var slug = await _dispatcher.Get(new GetTopicSlug
        {
            TopicId = command.TopicId
        });

        return Ok(slug.AsT0);
    }

    [Authorize]
    [HttpPost("pin-topic/{forumId}/{topicId}")]
    public async Task<ActionResult> PinTopic(Guid forumId, Guid topicId, [FromBody] bool pinned)
    {
        var command = new PinTopic
        {
            TopicId = topicId,
            ForumId = forumId,
            Pinned = pinned,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        var getPermissionsResult = await _dispatcher.Get(new GetPermissions 
        { 
            SiteId = CurrentSite.Id, 
            ForumId = forumId 
        });

        var permissions = getPermissionsResult.AsT0;

        var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !CurrentUser.IsSuspended;

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
        var command = new LockTopic
        {
            TopicId = topicId,
            ForumId = forumId,
            Locked = locked,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        var getPermissionsResult = await _dispatcher.Get(new GetPermissions 
        { 
            SiteId = CurrentSite.Id, 
            ForumId = forumId 
        });

        var permissions = getPermissionsResult.AsT0;

        var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions) && !CurrentUser.IsSuspended;

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
        var command = new DeleteTopic
        {
            TopicId = topicId,
            ForumId = forumId,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        var topicUserId = await _dbContext.Posts
            .Where(x =>
                x.Id == command.TopicId &&
                x.TopicId == null &&
                x.ForumId == command.ForumId &&
                x.Forum.Category.SiteId == command.SiteId &&
                x.Status != PostStatusType.Deleted)
            .Select(x => x.CreatedBy)
            .FirstOrDefaultAsync();

        var getPermissionsResult = await _dispatcher.Get(new GetPermissions
        {
            SiteId = CurrentSite.Id,
            ForumId = forumId
        });

        var permissions = getPermissionsResult.AsT0;

        var canDelete = _securityService.HasPermission(PermissionType.Delete, permissions);
        var canModerate = _securityService.HasPermission(PermissionType.Moderate, permissions);
        var authorized = (canDelete && topicUserId == CurrentUser.Id || canModerate) && !CurrentUser.IsSuspended;

        if (!authorized)
        {
            _logger.LogWarning("Unauthorized access to delete topic");
            return Unauthorized();
        }

        await _dispatcher.Send(command);

        return Ok();
    }
}
