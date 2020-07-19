using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Server.Caching;
using Atlas.Server.Data;
using Atlas.Server.Domain;
using Atlas.Server.Services;
using Atlas.Shared.Models.Admin.ForumGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    [Route("api/admin/forum-groups")]
    [ApiController]
    public class ForumGroupsController : ControllerBase
    {
        private readonly AtlasDbContext _dbContext;
        private readonly IContextService _contextService;
        private readonly ICacheManager _cacheManager;

        public ForumGroupsController(AtlasDbContext dbContext, IContextService contextService, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _contextService = contextService;
            _cacheManager = cacheManager;
        }

        [HttpGet("index-model")]
        public async Task<IndexModel> Index()
        {
            var result = new IndexModel();

            var site = await _contextService.CurrentSiteAsync();

            var forumGroups = await _dbContext.ForumGroups
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == site.Id && x.Status != StatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var forumGroup in forumGroups)
            {
                result.ForumGroups.Add(new IndexModel.ForumGroupModel
                {
                    Id = forumGroup.Id,
                    Name = forumGroup.Name,
                    SortOrder = forumGroup.SortOrder,
                    TotalTopics = forumGroup.TopicsCount,
                    TotalReplies = forumGroup.RepliesCount
                });
            }

            return result;
        }

        [HttpGet("create")]
        public async Task<FormModel> Create()
        {
            var result = new FormModel();

            var site = await _contextService.CurrentSiteAsync();

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == site.Id && x.Status != StatusType.Deleted)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new FormModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name
                });
            }

            return result;
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormModel.ForumGroupModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var forumGroupsCount = await _dbContext.ForumGroups
                .Where(x => x.SiteId == site.Id && x.Status != StatusType.Deleted)
                .CountAsync();

            var sortOrder = forumGroupsCount + 1;

            var forumGroup = new ForumGroup(site.Id,
                model.Name,
                sortOrder,
                model.PermissionSetId);

            _dbContext.ForumGroups.Add(forumGroup);
            _dbContext.Events.Add(new Event(nameof(ForumGroup), EventType.Created, forumGroup.Id, member.Id, new
            {
                forumGroup.SiteId,
                forumGroup.Name,
                forumGroup.SortOrder,
                forumGroup.PermissionSetId
            }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.ForumGroups(forumGroup.SiteId));

            return Ok();
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<FormModel>> Edit(Guid id)
        {
            var result = new FormModel();

            var forumGroup = await _dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == id && x.Status != StatusType.Deleted);

            if (forumGroup == null)
            {
                return NotFound();
            }

            result.ForumGroup = new FormModel.ForumGroupModel
            {
                Id = forumGroup.Id,
                Name = forumGroup.Name,
                PermissionSetId = forumGroup.PermissionSetId
            };

            var site = await _contextService.CurrentSiteAsync();

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == site.Id && x.Status != StatusType.Deleted)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new FormModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name
                });
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormModel.ForumGroupModel model)
        {
            var member = await _contextService.CurrentMemberAsync();

            var forumGroup = await _dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == model.Id && x.Status != StatusType.Deleted);

            if (forumGroup == null)
            {
                return NotFound();
            }

            forumGroup.UpdateDetails(model.Name, model.PermissionSetId);

            _dbContext.Events.Add(new Event(nameof(ForumGroup), EventType.Updated, forumGroup.Id, member.Id, new
            {
                forumGroup.Name,
                forumGroup.PermissionSetId
            }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.ForumGroups(forumGroup.SiteId));

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var currentMember = await _contextService.CurrentMemberAsync();

            var forumGroup = await _dbContext.ForumGroups.FirstOrDefaultAsync(x => x.Id == id && x.Status != StatusType.Deleted);

            if (forumGroup == null)
            {
                return NotFound();
            }

            forumGroup.Delete();
            _dbContext.Events.Add(new Event(nameof(ForumGroup), EventType.Deleted, forumGroup.Id, currentMember.Id));

            var forums = await _dbContext.Forums
                .Where(x => x.ForumGroupId == forumGroup.Id)
                .ToListAsync();

            foreach (var forum in forums)
            {
                forum.Delete();
                _dbContext.Events.Add(new Event(nameof(Forum), EventType.Deleted, forum.Id, currentMember.Id));
            }

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.ForumGroups(forumGroup.SiteId));

            return Ok();
        }
    }
}
