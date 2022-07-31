using System;
using System.Threading.Tasks;
using Atles.Commands.Categories;
using Atles.Core;
using Atles.Domain;
using Atles.Domain.Rules.Categories;
using Atles.Models.Admin.Categories;
using Atles.Queries.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/categories")]
public class CategoriesController : AdminControllerBase
{
    private readonly IDispatcher _dispatcher;

    public CategoriesController(IDispatcher dispatcher) : base(dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("list")]
    public async Task<ActionResult> List() => await ProcessGet(new GetCategoriesIndex { SiteId = CurrentSite.Id });

    [HttpGet("create")]
    public async Task<ActionResult> Create() => await ProcessGet(new GetCategoryForm { SiteId = CurrentSite.Id });

    [HttpPost("save")]
    public async Task<ActionResult> Save(FormComponentModel.CategoryModel model)
    {
        var command = new CreateCategory
        {
            Name = model.Name,
            PermissionSetId = model.PermissionSetId,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpGet("edit/{id}")]
    public async Task<ActionResult> Edit(Guid id) => await ProcessGet(new GetCategoryForm { SiteId = CurrentSite.Id, Id = id });

    [HttpPost("update")]
    public async Task<ActionResult> Update(FormComponentModel.CategoryModel model)
    {
        var command = new UpdateCategory
        {
            CategoryId = model.Id,
            Name = model.Name,
            PermissionSetId = model.PermissionSetId,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpPost("move-up")]
    public async Task<ActionResult> MoveUp([FromBody] Guid id)
    {
        var command = new MoveCategory
        {
            CategoryId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id,
            Direction = DirectionType.Up
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpPost("move-down")]
    public async Task<ActionResult> MoveDown([FromBody] Guid id)
    {
        var command = new MoveCategory
        {
            CategoryId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id,
            Direction = DirectionType.Down
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var command = new DeleteCategory
        {
            CategoryId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpGet("is-name-unique/{name}")]
    public async Task<ActionResult> IsNameUnique(string name) =>
        await ProcessGet(new IsCategoryNameUnique
        {
            SiteId = CurrentSite.Id,
            Name = name
        });

    [HttpGet("is-name-unique/{name}/{id}")]
    public async Task<ActionResult> IsNameUnique(string name, Guid id) =>
        await ProcessGet(new IsCategoryNameUnique
        {
            SiteId = CurrentSite.Id,
            Name = name,
            Id = id
        });
}
