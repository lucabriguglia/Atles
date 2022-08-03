using System;
using System.Threading.Tasks;
using Atles.Commands.Categories;
using Atles.Core;
using Atles.Domain;
using Atles.Models.Admin;
using Atles.Queries.Admin;
using Atles.Validators.ValidationRules;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/categories")]
public class CategoriesController : AdminControllerBase
{
    private readonly IDispatcher _dispatcher;
    private readonly ICategoryValidationRules _categoryValidationRules;
    private readonly IValidator<CategoryFormModel> _createValidator;

    public CategoriesController(
        IDispatcher dispatcher,
        ICategoryValidationRules categoryValidationRules, 
        IValidator<CategoryFormModel> createValidator) 
        : base(dispatcher)
    {
        _dispatcher = dispatcher;
        _categoryValidationRules = categoryValidationRules;
        _createValidator = createValidator;
    }

    [HttpGet("list")]
    public async Task<ActionResult> List()
    {
        return await ProcessGet(new GetCategoriesIndex
        {
            SiteId = CurrentSite.Id
        });
    }

    [HttpGet("create")]
    public async Task<ActionResult> Create()
    {
        return await ProcessGet(new GetCategoryForm
        {
            SiteId = CurrentSite.Id
        });
    }

    [HttpPost("save")]
    public async Task<ActionResult> Save(CategoryFormModel.CategoryModel model)
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
    public async Task<ActionResult> Edit(Guid id)
    {
        return await ProcessGet(new GetCategoryForm
        {
            SiteId = CurrentSite.Id, 
            Id = id
        });
    }

    [HttpPost("update")]
    public async Task<ActionResult> Update(CategoryFormModel.CategoryModel model)
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
    public async Task<ActionResult> IsNameUnique(string name)
    {
        var isNameUnique = await _categoryValidationRules.IsCategoryNameUnique(CurrentSite.Id, name);
        return Ok(isNameUnique);
    }

    [HttpGet("is-name-unique/{name}/{id}")]
    public async Task<ActionResult> IsNameUnique(string name, Guid id)
    {
        var isNameUnique = await _categoryValidationRules.IsCategoryNameUnique(CurrentSite.Id, name, id);
        return Ok(isNameUnique);
    }
}
