﻿using System;
using System.Threading.Tasks;
using Atles.Commands.Categories;
using Atles.Core;
using Atles.Domain;
using Atles.Domain.Rules.Categories;
using Atles.Reporting.Models.Admin.Categories;
using Atles.Reporting.Models.Admin.Categories.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/categories")]
    public class CategoriesController : AdminControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public CategoriesController(IDispatcher dispatcher) : base(dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet("list")]
        public async Task<IndexPageModel> List()
        {
            var site = await CurrentSite();

            var response = await _dispatcher.Get(new GetCategoriesIndex { SiteId = site.Id });

            return response;
        }

        [HttpGet("create")]
        public async Task<FormComponentModel> Create()
        {
            var site = await CurrentSite();

            var response = await _dispatcher.Get(new GetCategoryForm { SiteId = site.Id });

            return response;
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormComponentModel.CategoryModel model)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new CreateCategory
            {
                Name = model.Name,
                PermissionSetId = model.PermissionSetId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<FormComponentModel>> Edit(Guid id)
        {
            var site = await CurrentSite();

            var result = await _dispatcher.Get(new GetCategoryForm { SiteId = site.Id, Id = id });

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormComponentModel.CategoryModel model)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new UpdateCategory
            {
                CategoryId = model.Id,
                Name = model.Name,
                PermissionSetId = model.PermissionSetId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpPost("move-up")]
        public async Task<ActionResult> MoveUp([FromBody] Guid id)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new MoveCategory
            {
                CategoryId = id,
                SiteId = site.Id,
                UserId = user.Id,
                Direction = DirectionType.Up
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpPost("move-down")]
        public async Task<ActionResult> MoveDown([FromBody] Guid id)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new MoveCategory
            {
                CategoryId = id,
                SiteId = site.Id,
                UserId = user.Id,
                Direction = DirectionType.Down
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new DeleteCategory
            {
                CategoryId = id,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpGet("is-name-unique/{name}")]
        public async Task<IActionResult> IsNameUnique(string name)
        {
            var site = await CurrentSite();
            var isNameUnique = await _dispatcher.Get(new IsCategoryNameUnique 
            {
                SiteId = site.Id,
                Name = name
            });
            return Ok(isNameUnique);
        }

        [HttpGet("is-name-unique/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(string name, Guid id)
        {
            var site = await CurrentSite();
            var isNameUnique = await _dispatcher.Get(new IsCategoryNameUnique
            {
                SiteId = site.Id,
                Name = name,
                Id = id
            });
            return Ok(isNameUnique);
        }
    }
}
