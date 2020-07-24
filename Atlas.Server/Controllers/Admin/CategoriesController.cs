using System;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Domain.Categories;
using Atlas.Domain.Categories.Commands;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Atlas.Shared;
using Atlas.Shared.Admin.Categories;
using Atlas.Shared.Admin.Categories.Models;

namespace Atlas.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    [Route("api/admin/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly ICategoryService _forumGroupService;
        private readonly ICategoryRules _forumGroupRules;
        private readonly ICategoryModelBuilder _modelBuilder;

        public CategoriesController(IContextService contextService,
            ICategoryService forumGroupService,
            ICategoryRules forumGroupRules,
            ICategoryModelBuilder modelBuilder)
        {
            _contextService = contextService;
            _forumGroupService = forumGroupService;
            _forumGroupRules = forumGroupRules;
            _modelBuilder = modelBuilder;
        }

        [HttpGet("list")]
        public async Task<IndexModel> List()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildIndexModelAsync(site.Id);
        }

        [HttpGet("create")]
        public async Task<FormModel> Create()
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildFormModelAsync(site.Id);
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormModel.CategoryModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new CreateCategory
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
            var site = await _contextService.CurrentSiteAsync();

            var result = await _modelBuilder.BuildFormModelAsync(site.Id, id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormModel.CategoryModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new UpdateCategory
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

            var command = new MoveCategory
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

            var command = new MoveCategory
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

            var command = new DeleteCategory
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

        [HttpGet("is-name-unique/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(string name, Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _forumGroupRules.IsNameUniqueAsync(site.Id, name, id);
            return Ok(isNameUnique);
        }
    }
}
