﻿using System;
using System.Threading.Tasks;
using Atles.Domain;
using Atles.Domain.Categories;
using Atles.Domain.Categories.Commands;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Queries;
using Atles.Models.Admin.Categories;
using Atles.Reporting.Admin.Categories;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/categories")]
    public class CategoriesController : AdminControllerBase
    {
        private readonly IContextService _contextService;
        private readonly ICategoryRules _categoryRules;
        private readonly ICommandSender _commandSender;
        private readonly IQuerySender _querySender;

        public CategoriesController(IContextService contextService,
            ICategoryRules categoryRules,
            ICommandSender commandSender,
            IQuerySender querySender)
        {
            _contextService = contextService;
            _categoryRules = categoryRules;
            _commandSender = commandSender;
            _querySender = querySender;
        }

        [HttpGet("list")]
        public async Task<IndexPageModel> List()
        {
            var site = await _contextService.CurrentSiteAsync();

            var response = await _querySender.Send(new GetCategoriesIndex { SiteId = site.Id });

            return response;
        }

        [HttpGet("create")]
        public async Task<FormComponentModel> Create()
        {
            var site = await _contextService.CurrentSiteAsync();

            var response = await _querySender.Send(new GetCategoryForm { SiteId = site.Id });

            return response;
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormComponentModel.CategoryModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new CreateCategory
            {
                Name = model.Name,
                PermissionSetId = model.PermissionSetId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<FormComponentModel>> Edit(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();

            var result = await _querySender.Send(new GetCategoryForm { SiteId = site.Id, Id = id });

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormComponentModel.CategoryModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new UpdateCategory
            {
                Id = model.Id,
                Name = model.Name,
                PermissionSetId = model.PermissionSetId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpPost("move-up")]
        public async Task<ActionResult> MoveUp([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new MoveCategory
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id,
                Direction = Direction.Up
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpPost("move-down")]
        public async Task<ActionResult> MoveDown([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new MoveCategory
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id,
                Direction = Direction.Down
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new DeleteCategory
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _commandSender.Send(command);

            return Ok();
        }

        [HttpGet("is-name-unique/{name}")]
        public async Task<IActionResult> IsNameUnique(string name)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _categoryRules.IsNameUniqueAsync(site.Id, name);
            return Ok(isNameUnique);
        }

        [HttpGet("is-name-unique/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(string name, Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _categoryRules.IsNameUniqueAsync(site.Id, name, id);
            return Ok(isNameUnique);
        }
    }
}
