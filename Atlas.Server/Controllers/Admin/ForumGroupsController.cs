using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Server.Services;
using Atlas.Shared.Models.Admin.ForumGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Atlas.Domain.ForumGroups.Commands;
using Atlas.Domain.ForumGroups;

namespace Atlas.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    [Route("api/admin/forum-groups")]
    [ApiController]
    public class ForumGroupsController : ControllerBase
    {
        private readonly AtlasDbContext _dbContext;
        private readonly IContextService _contextService;
        private readonly IForumGroupService _forumGroupService;
        private readonly IForumGroupRules _forumGroupRules;

        public ForumGroupsController(AtlasDbContext dbContext, 
            IContextService contextService,
            IForumGroupService forumGroupService,
            IForumGroupRules forumGroupRules)
        {
            _dbContext = dbContext;
            _contextService = contextService;
            _forumGroupService = forumGroupService;
            _forumGroupRules = forumGroupRules;
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
            return await BuildFormModelAsync();
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormModel.ForumGroupModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new CreateForumGroup
            {
                Name = model.Name,
                PermissionSetId = model.PermissionSetId == Guid.Empty ? (Guid?)null : model.PermissionSetId,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _forumGroupService.CreateAsync(command);

            return Ok();
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<FormModel>> Edit(Guid id)
        {
            var result = await BuildFormModelAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormModel.ForumGroupModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new UpdateForumGroup
            {
                Id = model.Id,
                Name = model.Name,
                PermissionSetId = model.PermissionSetId == Guid.Empty ? (Guid?)null : model.PermissionSetId,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _forumGroupService.UpdateAsync(command);

            return Ok();
        }

        [HttpPost("move-up")]
        public async Task<ActionResult> MoveUp([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new MoveForumGroup
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id,
                Direction = Direction.Up
            };

            await _forumGroupService.MoveAsync(command);

            return Ok();
        }

        [HttpPost("move-down")]
        public async Task<ActionResult> MoveDown([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new MoveForumGroup
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id,
                Direction = Direction.Down
            };

            await _forumGroupService.MoveAsync(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new DeleteForumGroup
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _forumGroupService.DeleteAsync(command);

            return Ok();
        }

        [HttpGet("is-name-unique/{name}")]
        public async Task<IActionResult> IsNameUnique(string name)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _forumGroupRules.IsNameUniqueAsync(site.Id, name);
            return Ok(isNameUnique);
        }

        [HttpGet("is-name-unique/{id}/{name}")]
        public async Task<IActionResult> IsNameUnique(Guid id, string name)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _forumGroupRules.IsNameUniqueAsync(site.Id, id, name);
            return Ok(isNameUnique);
        }

        private async Task<FormModel> BuildFormModelAsync(Guid? id = null)
        {
            var result = new FormModel();

            var site = await _contextService.CurrentSiteAsync();

            if (id != null)
            {
                var forumGroup = await _dbContext.ForumGroups.FirstOrDefaultAsync(x => x.SiteId == site.Id && x.Id == id && x.Status != StatusType.Deleted);

                if (forumGroup == null)
                {
                    return null;
                }

                result.ForumGroup = new FormModel.ForumGroupModel
                {
                    Id = forumGroup.Id,
                    Name = forumGroup.Name,
                    PermissionSetId = forumGroup.PermissionSetId ?? Guid.Empty
                };
            }

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
    }
}
